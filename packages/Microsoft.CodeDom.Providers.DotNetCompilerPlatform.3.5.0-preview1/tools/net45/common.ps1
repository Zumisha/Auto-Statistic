# Copyright (c) .NET Foundation. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.


##
## Assigning a "DefaultValue" to a ParameterDescription will result in emitting this parameter when
## writing out a default compiler declaration.
##
## Setting IsRequired to $true will require the attribute to be set on all declarations in config.
##
Add-Type @"
	using System;
	
	public class CompilerParameterDescription {
		public string Name;
		public string DefaultValue;
		public bool IsRequired;
		public bool IsProviderOption;
	}

	public class CodeDomProviderDescription {
		public string TypeName;
		public string Assembly;
		public string Version;
		public string FileExtension;
		public CompilerParameterDescription[] Parameters;
	}
"@

function InstallCodeDomProvider($providerDescription) {
	##### Update/Rehydrate config declarations #####
	$config = ReadConfigFile
	$rehydratedCount = RehydrateOldDeclarations $config $providerDescription
	$updatedCount = UpdateDeclarations $config $providerDescription

	##### Add the default provider if it wasn't rehydrated above
	$defaultProvider = $config.xml.configuration["system.codedom"].compilers.compiler | where { $_.extension -eq $providerDescription.FileExtension }
	if ($defaultProvider -eq $null) { AddDefaultDeclaration $config $providerDescription }
	SaveConfigFile $config
}

function UninstallCodeDomProvider($providerType) {
	##### Dehydrate config declarations #####
	$config = ReadConfigFile
	DehydrateDeclarations $config $providerType | Out-Null
	SaveConfigFile $config
}

function GetConfigFileName() {
	# Try web.config first. Then fall back to app.config.
	$configFile = $project.ProjectItems | where { $_.Name -ieq "web.config" }
	if ($configFile -eq $null) { $configFile = $project.ProjectItems | where { $_.Name -ieq "app.config" } }
	$configPath = $configFile.Properties | where { $_.Name -ieq "LocalPath" }
    if ($configPath -eq $null) { $configPath = $configFile.Properties | where { $_.Name -ieq "FullPath" } }
	return $configPath.Value
}

function GetTempFileName() {
	$uname = $project.UniqueName
	if ([io.path]::IsPathRooted($uname)) { $uname = $project.Name }
	return [io.path]::Combine($env:TEMP, "Microsoft.CodeDom.Providers.DotNetCompilerPlatform.Temp", $uname + ".xml")
}

function ReadConfigFile() {
	$configFile = GetConfigFileName
	$configObj = @{ fileName = $configFile; xml = (Select-Xml -Path "$configFile" -XPath /).Node }
	$configObj.xml.PreserveWhitespace = $true
	return $configObj
}

function DehydrateDeclarations($config, $typeName) {
	$tempFile = GetTempFileName
	$xml
	$count = 0

	if ([io.file]::Exists($tempFile)) {
		$xml = (Select-Xml -Path "$tempFile" -XPath /).Node
		$xml.PreserveWhitespace = $true
	} else {
		$xml = New-Object System.Xml.XmlDocument
		$xml.PreserveWhitespace = $true
		$dd = $xml.CreateElement("driedDeclarations")
		$xml.AppendChild($dd) | Out-Null
	}

	foreach ($rec in $config.xml.configuration["system.codedom"].compilers.compiler  | where { IsSameType $_.type $typeName }) {
		# Remove records from config.
		$config.xml.configuration["system.codedom"].compilers.RemoveChild($rec) | Out-Null

		# Add the record to the temp stash. Don't worry about duplicates.
		AppendChildNode $xml.ImportNode($rec, $true) $xml.DocumentElement
		$count++
	}

	# Save the dehydrated declarations
	$tmpFolder = Split-Path $tempFile
	md -Force $tmpFolder | Out-Null
	$xml.Save($tempFile) | Out-Null
	return $count
}

function RehydrateOldDeclarations($config, $providerDescription) {
	$tempFile = GetTempFileName
	if (![io.file]::Exists($tempFile)) { return 0 }

	$count = 0
	$xml = (Select-Xml -Path "$tempFile" -XPath /).Node
	$xml.PreserveWhitespace = $true

	foreach($rec in $xml.driedDeclarations.compiler | where { IsSameType $_.type ($providerDescription.TypeName + "," + $providerDescription.Assembly) }) {
		# Remove records that match type, even if we don't end up rehydrating them.
		$xml.driedDeclarations.RemoveChild($rec) | Out-Null

		# Skip if an existing record of the same file extension already exists.
		$existingRecord = $config.xml.configuration["system.codedom"].compilers.compiler | where { $_.extension -eq $rec.extension }
		if ($existingRecord -ne $null) { continue }

		# Bring the record back to life
		AppendChildNode $config.xml.ImportNode($rec, $true) $config.xml.configuration["system.codedom"]["compilers"]
		$count++
	}

	# Make dried record removal permanent
	$xml.Save($tempFile) | Out-Null

	return $count
}

function UpdateDeclarations($config, $providerDescription) {
	$count = 0

	foreach ($provider in $config.xml.configuration["system.codedom"].compilers.compiler | where { IsSameType $_.type ($providerDescription.TypeName + "," + $providerDescription.Assembly) }) {
		# Count the existing declaration as found
		$count++

		# Update type
		$provider.type = "$($providerDescription.TypeName), $($providerDescription.Assembly), Version=$($providerDescription.Version), Culture=neutral, PublicKeyToken=31bf3856ad364e35"

		# Add default attributes if they are required and not already present
		foreach ($p in $providerDescription.Parameters | where { ($_.IsRequired -eq $true) -and ($_.IsProviderOption -eq $false) }) {
			if ($provider.($p.Name) -eq $null) {
				if ($p.DefaultValue -eq $null) {
					Write-Host "Failed to add parameter to '$($provider.name)' codeDom provider: '$($p.Name)' is required, but does not have a default value."
					return
				}
				$attr = $config.xml.CreateAttribute($p.Name)
				$attr.Value = $p.DefaultValue
				$provider.Attributes.InsertBefore($attr, $provider.Attributes["type"]) | Out-Null
			}
		}

		# Do the same thing for default providerOptions if not already present
		foreach ($p in $providerDescription.Parameters | where { ($_.IsRequired -eq $true) -and ($_.IsProviderOption -eq $true)}) {
			$existing = $provider.providerOption | where { $_.name -eq $p.Name }
			if ($existing -eq $null) {
				if ($p.DefaultValue -eq $null) {
					Write-Host "Failed to add providerOption to '$($provider.name)' codeDom provider: '$($p.Name)' is required, but does not have a default value."
					return
				}
				$po = $config.xml.CreateElement("providerOption")
				$po.SetAttribute("name", $p.Name)
				$po.SetAttribute("value", $p.DefaultValue)
				AppendChildNode $po $provider 4
			}
		}
	}

	return $count
}

function AddDefaultDeclaration($config, $providerDescription) {
	$dd = $config.xml.CreateElement("compiler")

	# file extension first
	$dd.SetAttribute("extension", $providerDescription.FileExtension)

	# everything else in the middle
	foreach ($p in $providerDescription.Parameters) {
		if ($p.IsRequired -and ($p.DefaultValue -eq $null)) {
			Write-Host "Failed to add default declaration for code dom extension '$($providerDescription.FileExtension)': '$($p.Name)' is required, but does not have a default value."
			return
		}

		if ($p.DefaultValue -ne $null) {
			if ($p.IsProviderOption -eq $true) {
				$po = $config.xml.CreateElement("providerOption")
				$po.SetAttribute("name", $p.Name)
				$po.SetAttribute("value", $p.DefaultValue)
				AppendChildNode $po $dd 4
			} else {
				$dd.SetAttribute($p.Name, $p.DefaultValue)
			}
		}
	}

	# type last
	$dd.SetAttribute("type", "$($providerDescription.TypeName), $($providerDescription.Assembly), Version=$($providerDescription.Version), Culture=neutral, PublicKeyToken=31bf3856ad364e35")

	AppendChildNode $dd $config.xml.configuration["system.codedom"]["compilers"]
}

function AppendChildNode($provider, $parent, $indentLevel = 3) {
	$lastSibling = $parent.ChildNodes | where { $_ -isnot [System.Xml.XmlWhitespace] } | select -Last 1
	if ($lastSibling -ne $null) {
		# If not the first child, then copy the whitespace convention of the existing child
		$ws = "";
		$prev = $lastSibling.PreviousSibling | where { $_ -is [System.Xml.XmlWhitespace] }
		while ($prev -ne $null) {
			$ws = $prev.data + $ws
			$prev = $prev.PreviousSibling | where { $_ -is [System.Xml.XmlWhitespace] }
		}
		$parent.InsertAfter($provider, $lastSibling) | Out-Null
		if ($ws.length -gt 0) { $parent.InsertAfter($parent.OwnerDocument.CreateWhitespace($ws), $lastSibling) | Out-Null }
		return
	}

	# Add on a new line with indents. Make sure there is no existing whitespace mucking this up.
	foreach ($exws in $parent.ChildNodes | where { $_ -is [System.Xml.XmlWhitespace] }) { $parent.RemoveChild($exws) }
	$parent.AppendChild($parent.OwnerDocument.CreateWhitespace("`r`n")) | Out-Null
	$parent.AppendChild($parent.OwnerDocument.CreateWhitespace("  " * $indentLevel)) | Out-Null
	$parent.AppendChild($provider) | Out-Null
	$parent.AppendChild($parent.OwnerDocument.CreateWhitespace("`r`n")) | Out-Null
	$parent.AppendChild($parent.OwnerDocument.CreateWhitespace("  " * ($indentLevel - 1))) | Out-Null
}

function SaveConfigFile($config) {
	$config.xml.Save($config.fileName)
}

function IsSameType($typeString1, $typeString2) {

	if (($typeString1 -eq $null) -or ($typeString2 -eq $null)) { return $false }

	# First check the type
	$t1 = $typeString1.Split(',')[0].Trim()
	$t2 = $typeString2.Split(',')[0].Trim()
	if ($t1 -cne $t2) { return $false }

	# Then check for assembly match if possible
	$a1 = $typeString1.Split(',')[1]
	$a2 = $typeString2.Split(',')[1]
	if (($a1 -ne $null) -and ($a2 -ne $null)) {
		return ($a1.Trim() -eq $a2.Trim())
	}

	# Don't care about assembly. Match is good.
	return $true
}

# SIG # Begin signature block
# MIIjlQYJKoZIhvcNAQcCoIIjhjCCI4ICAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCAKaw8D28PUyqaj
# 8cg1N507irkPJtijF01kKz3vbQKlfqCCDYUwggYDMIID66ADAgECAhMzAAABiK9S
# 1rmSbej5AAAAAAGIMA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTEwHhcNMjAwMzA0MTgzOTQ4WhcNMjEwMzAzMTgzOTQ4WjB0MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYDVQQDExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQCSCNryE+Cewy2m4t/a74wZ7C9YTwv1PyC4BvM/kSWPNs8n0RTe+FvYfU+E9uf0
# t7nYlAzHjK+plif2BhD+NgdhIUQ8sVwWO39tjvQRHjP2//vSvIfmmkRoML1Ihnjs
# 9kQiZQzYRDYYRp9xSQYmRwQjk5hl8/U7RgOiQDitVHaU7BT1MI92lfZRuIIDDYBd
# vXtbclYJMVOwqZtv0O9zQCret6R+fRSGaDNfEEpcILL+D7RV3M4uaJE4Ta6KAOdv
# V+MVaJp1YXFTZPKtpjHO6d9pHQPZiG7NdC6QbnRGmsa48uNQrb6AfmLKDI1Lp31W
# MogTaX5tZf+CZT9PSuvjOCLNAgMBAAGjggGCMIIBfjAfBgNVHSUEGDAWBgorBgEE
# AYI3TAgBBggrBgEFBQcDAzAdBgNVHQ4EFgQUj9RJL9zNrPcL10RZdMQIXZN7MG8w
# VAYDVR0RBE0wS6RJMEcxLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5kIE9wZXJh
# dGlvbnMgTGltaXRlZDEWMBQGA1UEBRMNMjMwMDEyKzQ1ODM4NjAfBgNVHSMEGDAW
# gBRIbmTlUAXTgqoXNzcitW2oynUClTBUBgNVHR8ETTBLMEmgR6BFhkNodHRwOi8v
# d3d3Lm1pY3Jvc29mdC5jb20vcGtpb3BzL2NybC9NaWNDb2RTaWdQQ0EyMDExXzIw
# MTEtMDctMDguY3JsMGEGCCsGAQUFBwEBBFUwUzBRBggrBgEFBQcwAoZFaHR0cDov
# L3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9jZXJ0cy9NaWNDb2RTaWdQQ0EyMDEx
# XzIwMTEtMDctMDguY3J0MAwGA1UdEwEB/wQCMAAwDQYJKoZIhvcNAQELBQADggIB
# ACnXo8hjp7FeT+H6iQlV3CcGnkSbFvIpKYafgzYCFo3UHY1VHYJVb5jHEO8oG26Q
# qBELmak6MTI+ra3WKMTGhE1sEIlowTcp4IAs8a5wpCh6Vf4Z/bAtIppP3p3gXk2X
# 8UXTc+WxjQYsDkFiSzo/OBa5hkdW1g4EpO43l9mjToBdqEPtIXsZ7Hi1/6y4gK0P
# mMiwG8LMpSn0n/oSHGjrUNBgHJPxgs63Slf58QGBznuXiRaXmfTUDdrvhRocdxIM
# i8nXQwWACMiQzJSRzBP5S2wUq7nMAqjaTbeXhJqD2SFVHdUYlKruvtPSwbnqSRWT
# GI8s4FEXt+TL3w5JnwVZmZkUFoioQDMMjFyaKurdJ6pnzbr1h6QW0R97fWc8xEIz
# LIOiU2rjwWAtlQqFO8KNiykjYGyEf5LyAJKAO+rJd9fsYR+VBauIEQoYmjnUbTXM
# SY2Lf5KMluWlDOGVh8q6XjmBccpaT+8tCfxpaVYPi1ncnwTwaPQvVq8RjWDRB7Pa
# 8ruHgj2HJFi69+hcq7mWx5nTUtzzFa7RSZfE5a1a5AuBmGNRr7f8cNfa01+tiWjV
# Kk1a+gJUBSP0sIxecFbVSXTZ7bqeal45XSDIisZBkWb+83TbXdTGMDSUFKTAdtC+
# r35GfsN8QVy59Hb5ZYzAXczhgRmk7NyE6jD0Ym5TKiW5MIIHejCCBWKgAwIBAgIK
# YQ6Q0gAAAAAAAzANBgkqhkiG9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNV
# BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jv
# c29mdCBDb3Jwb3JhdGlvbjEyMDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlm
# aWNhdGUgQXV0aG9yaXR5IDIwMTEwHhcNMTEwNzA4MjA1OTA5WhcNMjYwNzA4MjEw
# OTA5WjB+MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSgwJgYD
# VQQDEx9NaWNyb3NvZnQgQ29kZSBTaWduaW5nIFBDQSAyMDExMIICIjANBgkqhkiG
# 9w0BAQEFAAOCAg8AMIICCgKCAgEAq/D6chAcLq3YbqqCEE00uvK2WCGfQhsqa+la
# UKq4BjgaBEm6f8MMHt03a8YS2AvwOMKZBrDIOdUBFDFC04kNeWSHfpRgJGyvnkmc
# 6Whe0t+bU7IKLMOv2akrrnoJr9eWWcpgGgXpZnboMlImEi/nqwhQz7NEt13YxC4D
# dato88tt8zpcoRb0RrrgOGSsbmQ1eKagYw8t00CT+OPeBw3VXHmlSSnnDb6gE3e+
# lD3v++MrWhAfTVYoonpy4BI6t0le2O3tQ5GD2Xuye4Yb2T6xjF3oiU+EGvKhL1nk
# kDstrjNYxbc+/jLTswM9sbKvkjh+0p2ALPVOVpEhNSXDOW5kf1O6nA+tGSOEy/S6
# A4aN91/w0FK/jJSHvMAhdCVfGCi2zCcoOCWYOUo2z3yxkq4cI6epZuxhH2rhKEmd
# X4jiJV3TIUs+UsS1Vz8kA/DRelsv1SPjcF0PUUZ3s/gA4bysAoJf28AVs70b1FVL
# 5zmhD+kjSbwYuER8ReTBw3J64HLnJN+/RpnF78IcV9uDjexNSTCnq47f7Fufr/zd
# sGbiwZeBe+3W7UvnSSmnEyimp31ngOaKYnhfsi+E11ecXL93KCjx7W3DKI8sj0A3
# T8HhhUSJxAlMxdSlQy90lfdu+HggWCwTXWCVmj5PM4TasIgX3p5O9JawvEagbJjS
# 4NaIjAsCAwEAAaOCAe0wggHpMBAGCSsGAQQBgjcVAQQDAgEAMB0GA1UdDgQWBBRI
# bmTlUAXTgqoXNzcitW2oynUClTAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMAQTAL
# BgNVHQ8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBRyLToCMZBD
# uRQFTuHqp8cx0SOJNDBaBgNVHR8EUzBRME+gTaBLhklodHRwOi8vY3JsLm1pY3Jv
# c29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFf
# MDNfMjIuY3JsMF4GCCsGAQUFBwEBBFIwUDBOBggrBgEFBQcwAoZCaHR0cDovL3d3
# dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFf
# MDNfMjIuY3J0MIGfBgNVHSAEgZcwgZQwgZEGCSsGAQQBgjcuAzCBgzA/BggrBgEF
# BQcCARYzaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9kb2NzL3ByaW1h
# cnljcHMuaHRtMEAGCCsGAQUFBwICMDQeMiAdAEwAZQBnAGEAbABfAHAAbwBsAGkA
# YwB5AF8AcwB0AGEAdABlAG0AZQBuAHQALiAdMA0GCSqGSIb3DQEBCwUAA4ICAQBn
# 8oalmOBUeRou09h0ZyKbC5YR4WOSmUKWfdJ5DJDBZV8uLD74w3LRbYP+vj/oCso7
# v0epo/Np22O/IjWll11lhJB9i0ZQVdgMknzSGksc8zxCi1LQsP1r4z4HLimb5j0b
# pdS1HXeUOeLpZMlEPXh6I/MTfaaQdION9MsmAkYqwooQu6SpBQyb7Wj6aC6VoCo/
# KmtYSWMfCWluWpiW5IP0wI/zRive/DvQvTXvbiWu5a8n7dDd8w6vmSiXmE0OPQvy
# CInWH8MyGOLwxS3OW560STkKxgrCxq2u5bLZ2xWIUUVYODJxJxp/sfQn+N4sOiBp
# mLJZiWhub6e3dMNABQamASooPoI/E01mC8CzTfXhj38cbxV9Rad25UAqZaPDXVJi
# hsMdYzaXht/a8/jyFqGaJ+HNpZfQ7l1jQeNbB5yHPgZ3BtEGsXUfFL5hYbXw3MYb
# BL7fQccOKO7eZS/sl/ahXJbYANahRr1Z85elCUtIEJmAH9AAKcWxm6U/RXceNcbS
# oqKfenoi+kiVH6v7RyOA9Z74v2u3S5fi63V4GuzqN5l5GEv/1rMjaHXmr/r8i+sL
# gOppO6/8MO0ETI7f33VtY5E90Z1WTk+/gFcioXgRMiF670EKsT/7qMykXcGhiJtX
# cVZOSEXAQsmbdlsKgEhr/Xmfwb1tbWrJUnMTDXpQzTGCFWYwghViAgEBMIGVMH4x
# CzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRt
# b25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01p
# Y3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBIDIwMTECEzMAAAGIr1LWuZJt6PkAAAAA
# AYgwDQYJYIZIAWUDBAIBBQCgga4wGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQw
# HAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwLwYJKoZIhvcNAQkEMSIEIL5x
# 0or5GQPS6af3tBiDwBk2UUOeG6wM9iNlhfHGNf+1MEIGCisGAQQBgjcCAQwxNDAy
# oBSAEgBNAGkAYwByAG8AcwBvAGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5j
# b20wDQYJKoZIhvcNAQEBBQAEggEATGapXiAQNux1fvxhV7On0D6F41Wxiek4L+4d
# r0oeClirnH/LS/x/BIo/5jxTX7PSCBO+RoCjnX1UzVk+eXHLVt0ah0y2XktYBjI0
# 7hvKQg2oC24LWbMNbRqNV/NuhXBQqPBrRJS8pd0FEbg4AclzmBWyvE3OAlYOouy3
# A0M4Tj+w5hgg0MRTQ1XCyg5QeGvrmHg+NSejBQrCgatwhB4zr8QkARjyx7n6OBpj
# JNQrsDog02X9mxJglQqBtNw2wqiOVjoZ6yOBJu50I5chgN7jrDZOkiNHzhj71G9O
# xCcghyt+DSxsxNWT4kOeUijaIBazfNeQgH2CrWjabbDBU96rR6GCEvAwghLsBgor
# BgEEAYI3AwMBMYIS3DCCEtgGCSqGSIb3DQEHAqCCEskwghLFAgEDMQ8wDQYJYIZI
# AWUDBAIBBQAwggFUBgsqhkiG9w0BCRABBKCCAUMEggE/MIIBOwIBAQYKKwYBBAGE
# WQoDATAxMA0GCWCGSAFlAwQCAQUABCAIxnYmZyKGatNcxpWyN2p/5x8E5+Oxdr7E
# usWmDSl8kwIGXqba08KSGBIyMDIwMDUxMTIwNDgwMS41N1owBIACAfSggdSkgdEw
# gc4xCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdS
# ZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKTAnBgNVBAsT
# IE1pY3Jvc29mdCBPcGVyYXRpb25zIFB1ZXJ0byBSaWNvMSYwJAYDVQQLEx1UaGFs
# ZXMgVFNTIEVTTjoyMTY0LUUzNUMtMjA0NDElMCMGA1UEAxMcTWljcm9zb2Z0IFRp
# bWUtU3RhbXAgU2VydmljZaCCDkQwggT1MIID3aADAgECAhMzAAABKbBXVguv9Zf7
# AAAAAAEpMA0GCSqGSIb3DQEBCwUAMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpX
# YXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQg
# Q29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAy
# MDEwMB4XDTE5MTIxOTAxMTUwMVoXDTIxMDMxNzAxMTUwMVowgc4xCzAJBgNVBAYT
# AlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYD
# VQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKTAnBgNVBAsTIE1pY3Jvc29mdCBP
# cGVyYXRpb25zIFB1ZXJ0byBSaWNvMSYwJAYDVQQLEx1UaGFsZXMgVFNTIEVTTjoy
# MTY0LUUzNUMtMjA0NDElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2Vy
# dmljZTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBANqDHp2yrUpA8rxy
# 8VAxmH2Qw35Q888dsgeY5SwLz74OWo6OjJktac9Y0fUxHYatLnpZm28GYtly7YZG
# hLvcsJNPVWOx/NqsN2RSVudPdmxDU6ohMI0TznF0OYGIWMwGpmlQR3PXHOBQXJF6
# xFi3c02iaWPFCm4zR9OD4bu3cTSFhqDrJWNXAtwziq2SDrEesctrpSiqmRfmd/sr
# zj52BjU+wIU+Dc01i1hhGpZMFwbXlgeLvyLsAhK12V9jLEz3NVPgIk5DsFVwBNhU
# 0YygVUzHbQed2zRb/hHWVWJmwRre9C75nGqdDE0RnXCTLxArY1nnmYzi80VMSs8v
# emoFzR0CAwEAAaOCARswggEXMB0GA1UdDgQWBBSNVD891Zftxh8IDZ3PosfXi2JC
# jzAfBgNVHSMEGDAWgBTVYzpcijGQ80N7fEYbxTNoWoVtVTBWBgNVHR8ETzBNMEug
# SaBHhkVodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9N
# aWNUaW1TdGFQQ0FfMjAxMC0wNy0wMS5jcmwwWgYIKwYBBQUHAQEETjBMMEoGCCsG
# AQUFBzAChj5odHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY1Rp
# bVN0YVBDQV8yMDEwLTA3LTAxLmNydDAMBgNVHRMBAf8EAjAAMBMGA1UdJQQMMAoG
# CCsGAQUFBwMIMA0GCSqGSIb3DQEBCwUAA4IBAQAIqO6y0BzX/O3qfhUDMazBrKw2
# yf7Ny3Fe4gkar5fsKeSeNbzSK4Ai4ZX8UvHxwD6gZf0M7wJHYheW0jQM5BVqP5T2
# sjOHV35E7WmnsrgUVEiJ3ZiKGaEi6PMzOf78Y5fqhQKtW9HdReu20BcUL28e6euR
# lio3QKoOd7XEunswalA7nb3hMGRc2cw1Ev3kbqzE3lsHITlyFNWJMhjYQiLKE2ne
# FPyFYUOBEWTVJkvpoAcYu+0EjsVtrWzLecOhzBV54zyGimTEC7KUD+suE7csvKpW
# 8EAo8zi9HzgJa/X+4xOHa1Xp+YhwQXVcgIgeyBlgL6WPFJgIrE0k5HAW8YS5MIIG
# cTCCBFmgAwIBAgIKYQmBKgAAAAAAAjANBgkqhkiG9w0BAQsFADCBiDELMAkGA1UE
# BhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAc
# BgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEyMDAGA1UEAxMpTWljcm9zb2Z0
# IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IDIwMTAwHhcNMTAwNzAxMjEzNjU1
# WhcNMjUwNzAxMjE0NjU1WjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGlu
# Z3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBv
# cmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDCC
# ASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKkdDbx3EYo6IOz8E5f1+n9p
# lGt0VBDVpQoAgoX77XxoSyxfxcPlYcJ2tz5mK1vwFVMnBDEfQRsalR3OCROOfGEw
# WbEwRA/xYIiEVEMM1024OAizQt2TrNZzMFcmgqNFDdDq9UeBzb8kYDJYYEbyWEeG
# MoQedGFnkV+BVLHPk0ySwcSmXdFhE24oxhr5hoC732H8RsEnHSRnEnIaIYqvS2SJ
# UGKxXf13Hz3wV3WsvYpCTUBR0Q+cBj5nf/VmwAOWRH7v0Ev9buWayrGo8noqCjHw
# 2k4GkbaICDXoeByw6ZnNPOcvRLqn9NxkvaQBwSAJk3jN/LzAyURdXhacAQVPIk0C
# AwEAAaOCAeYwggHiMBAGCSsGAQQBgjcVAQQDAgEAMB0GA1UdDgQWBBTVYzpcijGQ
# 80N7fEYbxTNoWoVtVTAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMAQTALBgNVHQ8E
# BAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBTV9lbLj+iiXGJo0T2U
# kFvXzpoYxDBWBgNVHR8ETzBNMEugSaBHhkVodHRwOi8vY3JsLm1pY3Jvc29mdC5j
# b20vcGtpL2NybC9wcm9kdWN0cy9NaWNSb29DZXJBdXRfMjAxMC0wNi0yMy5jcmww
# WgYIKwYBBQUHAQEETjBMMEoGCCsGAQUFBzAChj5odHRwOi8vd3d3Lm1pY3Jvc29m
# dC5jb20vcGtpL2NlcnRzL01pY1Jvb0NlckF1dF8yMDEwLTA2LTIzLmNydDCBoAYD
# VR0gAQH/BIGVMIGSMIGPBgkrBgEEAYI3LgMwgYEwPQYIKwYBBQUHAgEWMWh0dHA6
# Ly93d3cubWljcm9zb2Z0LmNvbS9QS0kvZG9jcy9DUFMvZGVmYXVsdC5odG0wQAYI
# KwYBBQUHAgIwNB4yIB0ATABlAGcAYQBsAF8AUABvAGwAaQBjAHkAXwBTAHQAYQB0
# AGUAbQBlAG4AdAAuIB0wDQYJKoZIhvcNAQELBQADggIBAAfmiFEN4sbgmD+BcQM9
# naOhIW+z66bM9TG+zwXiqf76V20ZMLPCxWbJat/15/B4vceoniXj+bzta1RXCCtR
# gkQS+7lTjMz0YBKKdsxAQEGb3FwX/1z5Xhc1mCRWS3TvQhDIr79/xn/yN31aPxzy
# mXlKkVIArzgPF/UveYFl2am1a+THzvbKegBvSzBEJCI8z+0DpZaPWSm8tv0E4XCf
# Mkon/VWvL/625Y4zu2JfmttXQOnxzplmkIz/amJ/3cVKC5Em4jnsGUpxY517IW3D
# nKOiPPp/fZZqkHimbdLhnPkd/DjYlPTGpQqWhqS9nhquBEKDuLWAmyI4ILUl5WTs
# 9/S/fmNZJQ96LjlXdqJxqgaKD4kWumGnEcua2A5HmoDF0M2n0O99g/DhO3EJ3110
# mCIIYdqwUB5vvfHhAN/nMQekkzr3ZUd46PioSKv33nJ+YWtvd6mBy6cJrDm77MbL
# 2IK0cs0d9LiFAR6A+xuJKlQ5slvayA1VmXqHczsI5pgt6o3gMy4SKfXAL1QnIffI
# rE7aKLixqduWsqdCosnPGUFN4Ib5KpqjEWYw07t0MkvfY3v1mYovG8chr1m1rtxE
# PJdQcdeh0sVV42neV8HR3jDA/czmTfsNv11P6Z0eGTgvvM9YBS7vDaBQNdrvCScc
# 1bN+NR4Iuto229Nfj950iEkSoYIC0jCCAjsCAQEwgfyhgdSkgdEwgc4xCzAJBgNV
# BAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4w
# HAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKTAnBgNVBAsTIE1pY3Jvc29m
# dCBPcGVyYXRpb25zIFB1ZXJ0byBSaWNvMSYwJAYDVQQLEx1UaGFsZXMgVFNTIEVT
# TjoyMTY0LUUzNUMtMjA0NDElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAg
# U2VydmljZaIjCgEBMAcGBSsOAwIaAxUAszJss/RCOvFe5naS3VKAV40iLJeggYMw
# gYCkfjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYD
# VQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDANBgkqhkiG9w0BAQUF
# AAIFAOJjzicwIhgPMjAyMDA1MTExNzE0MTVaGA8yMDIwMDUxMjE3MTQxNVowdzA9
# BgorBgEEAYRZCgQBMS8wLTAKAgUA4mPOJwIBADAKAgEAAgIr7AIB/zAHAgEAAgIR
# 3TAKAgUA4mUfpwIBADA2BgorBgEEAYRZCgQCMSgwJjAMBgorBgEEAYRZCgMCoAow
# CAIBAAIDB6EgoQowCAIBAAIDAYagMA0GCSqGSIb3DQEBBQUAA4GBADEs/9is0UE7
# TXsoFCVFLtg0CXJJnXztYhe0zkUSExRKCyXfdwazIx8eXOznKb3ieVYbUErWIZSL
# YRnc8ACVqCpFS6EzA3HaQaTZn0VXL8PMiWcs5+oLcBWjZj7CAyyGhglGkVZNBv7z
# z3orvvidKDPGq3witoBBcWoUW2VedoM3MYIDDTCCAwkCAQEwgZMwfDELMAkGA1UE
# BhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAc
# BgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0
# IFRpbWUtU3RhbXAgUENBIDIwMTACEzMAAAEpsFdWC6/1l/sAAAAAASkwDQYJYIZI
# AWUDBAIBBQCgggFKMBoGCSqGSIb3DQEJAzENBgsqhkiG9w0BCRABBDAvBgkqhkiG
# 9w0BCQQxIgQg6Lpa0PY6jo0n1Ag/+4Gaqn8J4f6lWLvu6oTJsdVq1zYwgfoGCyqG
# SIb3DQEJEAIvMYHqMIHnMIHkMIG9BCC+/mA+550QK0p/+/axDOKZ6H4y1tItpav+
# PTVR/d8lBjCBmDCBgKR+MHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5n
# dG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9y
# YXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMz
# AAABKbBXVguv9Zf7AAAAAAEpMCIEIFM/KJLyjmIRcUaCv1n0pKBUXNgImPl/DrqN
# Dqy7HvroMA0GCSqGSIb3DQEBCwUABIIBACn7jI4fw/azVSL/XhMjPRx3kl+J5JBc
# G9qT5swJ5EMGZyFH4pAeUNz1FbFjk+oky9QWVj/aV6b6vCjnT9TJ+Z64pjsQLLDw
# dZPwdE16sb8oqY1oU0ElE0m4A4iaJU3UmxSlJW34W86RnEYFF5ejj1mU/H/UxVgy
# P0rvdoaN9Cvs6iyfwLs59l5eiPZH2qRwyLUzHdPxLtotp6TLqhRs3DSZslalpMNQ
# PLQBd52+bMFRkcgiFnZ2jVx02riMoKKC/+bTKio8Jj8Pq/0AZwRyonxRs+cp8VmV
# EvK+8QKJyYN84V0vFBKPGl+TkysCRi8G4m/ncxlBSiZZpWOIbVDhKpA=
# SIG # End signature block
