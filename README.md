# Auto-Statistic
Решение для автоматизации сбора статистики выполнения программ.

Позволяет производить анализ работы программ и упрощает сравнение эффективности различных версий.

# ![Main](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Main.png)

### Минимальное влияние на результаты измерений
# ![ResourcesUsage](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/ResourcesUsage.png)

### Может работать как с самостоятельными программами, так и с интерпретируемыми
Для переключения режима работы используется переключатель в верхнем левом углу окна.

# ![Main2](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Main2.png)

С помощью кнопок "Добавить исполняемые файлы", "Добавить файлы программ" можно добавлять исполняемые файлы и файлы с кодом программ, соответственно. С помощью кнопок "Очистить" - очищать списки. 

Файлы с кодом программ будут указаны в качестве первого позиционного аргумента командной строки при запуске исполняемого файла.

# ![Interpreter](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Interpreter.png)

### Быстрая генерация наборов параметров запуска
В средней части интерфейса находятся элементы для задания наборов параметров запуска программ и значений, используемых для проверки корректности результата выполнения тестируемой программы.

# ![Replace](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Replace.png)

С помощью функции "заменить слово на диапазон" можно быстро генерировать требуемые наборы параметров запуска.

# ![ReplaceResult](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/ReplaceResult.png)

### Гибкая система проверки результата выполнения
С помощью кнопки верхнего меню можно задать алгоритм проверки результата выполнения тестируемых программ.

# ![CheckAlg](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/CheckAlg.png)

Алгоритм задаётся при помощи C# кода. Если код некорректен, будет выведено сообщение с описанием проблемы.

По умолчанию предлагается проверка, что вывод не содержит слова "Error" и содержит значение, указанное в таблице.

# ![CompileError](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/CompileError.png)

### Настройка процесса сбора статистики
* Решение позволяет предотвращать сбор некорректной статистики вследствие исчерпания тестируемой программой ресурса оперативной памяти.
* Позволяет ограничить максимальное время выполнения программы (если указан 0 - не ограничено).
* Позволяет выполнять запуск программы с заданным набором параметров, пока не будет достигнута требуемая точность значения времени выполнения (если указан 0 - выполнять пока не будет достигнут указанный лимит запусков).
* Позволяет ограничить максимальное количество запусков тестируемой программы с одним набором параметров.

# ![Restricts](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Restricts.png)

### Контроль процесса сбора статистики
В нижней части окна находятся кнопки для запуска и прерывания процесса сбора статистики.

А также информационная панель, на которую выводится прогресс сбора статистики, использование программой процессорного времени и оперативной памяти, номер набора данных, номер итерация и имя выполняемой в данный момент программы.

# ![Status](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Status.png)

### Профили тестирования
Решение позволяет сохранять и загружать конфигурации тестирования.

При запуске решения загружается конфигурация, которая использовалась на момент окончания прошлого использования решения.

# ![Save](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Save.png)

### Удобное представление собранной статистики
После завершения процесса сбора статистики будет открыта папка с собранной статистикой.

# ![Ending](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Ending.png)
* В файле "Results" собирается общая статистика выполнения программ (берётся среднее значение времени и худшее значение используемой памяти из результатов всех итераций программы с данным набором данных).
# ![Results](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Results.png)

* В папке "Logs" сохраняется вывод программ (отдельный файл на каждую программу, различные параметры и итерации заметно выделяются в файле).
# ![LogFolder](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/LogFolder.png)
# ![Log](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Log.png)
* В папке "Profiller" сохраняются файлы со значениями используемых тестируемой программой ресурсов в разные моменты времени её работы (на каждую программу, набор данных и итерацию свой файл)
# ![ProfilerFolder](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/ProfilerFolder.png)
# ![Profiler](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/Profiler.png)
* В файле "System-Info" приводятся некоторые характеристики системы, на которой производится тестирование.
# ![SystemInfo](https://raw.githubusercontent.com/Zumisha/Auto-Statistic/master/images/SystemInfo.png)
