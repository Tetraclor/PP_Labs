﻿using System;

namespace Lab1ConsoleApp
{
    class Program
    {
        static BusinessLogic logic;

        static void Main(string[] args)
        {
            // 
            IDataSource dataSource = new MemoryDataSource();
            logic = new BusinessLogic(dataSource);


            // Заносим первоначальные данные для тестов
            logic.Save(new SampleEmployeeRecord("Александр Ф. Ю.;Разработчик;Разработка;50000"));
            logic.Save(new TempWorkerRecord("Михаил А. Я.;DevOps;Разработка;100000;20.09.2022"));
            logic.Save(new TraineeRecord("Дмитрий Ж. Т.;Бухгалтер;Бухгалтерия;30000;УДГУ"));

            bool exit = false;
            while (!exit)
            {

                // Цикл меню, печатаем меню, читаем команду, в зависимости от команды вызываем метод
                PrintMenu();
                string command = Console.ReadLine();
                switch (command)
                {
                    case "1":
                        AddRecord();
                        break;
                    case "2":
                        PrintList();
                        break;
                    case "3":
                        ChangeRecord();
                        break;
                    case "4":
                        DeleteRecord();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }
        }

        /// <summary>
        /// Печать меню навигаации программы
        /// </summary>
        static void PrintMenu()
        {
            var menu = @"
1) Добавить запись (выбор подвида записи и ввод данных)
2) Просмотреть записи (список записей, отсортированных по заданному критерию)
3) Изменить запись (поиск записи по номеру/идентификатору и ввод измененной
версии записи)
4) Удалить запись (поиск записи по номеру/идентификатору и подтверждение
удаления)
5) Выход. Выход из программы
";
            Console.Write(menu);
        }

        /// <summary>
        /// Метод добавления новой записи
        /// </summary>
        static void AddRecord()
        {
            var ans = GetNumberAnswer("Выбрать подвид записи: \n1)Обычный сотрудник\n2)Временный работник\n3)Стажер", 3);
            if (ans == -1) return;

            AddOrUpdateFromConsole(ans);
        }

        /// <summary>
        /// Считывает данные из консоли создает объект по переданному типу recordType и сохраняет объект по id
        /// </summary>
        /// <param name="recordType"></param>
        /// <param name="id"></param>
        static void AddOrUpdateFromConsole(int recordType, int id = 0)
        {
            Console.Write("Введите через точку с запятой значения следующих полей: ФИО;Должность;Отдел;Оклад");
            EmployeeRecord record = null;
            try
            {
                if (recordType == 1)
                {
                    Console.WriteLine();
                    record = new SampleEmployeeRecord(Console.ReadLine());
                }
                else if (recordType == 2)
                {
                    Console.WriteLine(";Дата оконачания договора");
                    record = new TempWorkerRecord(Console.ReadLine());
                }
                else if (recordType == 3)
                {
                    Console.WriteLine(";Учебное заведение");
                    record = new TraineeRecord(Console.ReadLine());
                }
                record.id = id;
                Console.WriteLine($"Добавлена запись:\n{logic.Save(record)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка заполнения данных: " + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Метот вывода списка всех записей
        /// </summary>
        static void PrintList()
        {
            var records = logic.GetAll();
            if(records.Count == 0)
            {
                Console.WriteLine("Записей нет");
                return;
            }
            var str = string.Join("\n", records);
            Console.WriteLine(str);
        }

        /// <summary>
        /// Метод измененияя записи, запрашивается id, если существует то обновляет запись по введенным данным с консоли
        /// </summary>
        static void ChangeRecord()
        {
            var id = GetNumberAnswer("Изменение записи, введите идентификатор записи", int.MaxValue);
            if (id == -1) return;
            var record = logic.Get(id);
            if (record == null)
            {
                Console.WriteLine("По переданному идентификатору запись не найдена");
                return;
            }
            Console.WriteLine($"Изменение записи:{record}");
            if (record is SampleEmployeeRecord) AddOrUpdateFromConsole(0, id);
            else if (record is TempWorkerRecord) AddOrUpdateFromConsole(1, id);
            else if (record is TraineeRecord) AddOrUpdateFromConsole(2, id);
        }

        /// <summary>
        /// Метод удаления записи запрашивается id если существует запись с таким id то удалется по потверждению пользователя
        /// </summary>
        static void DeleteRecord()
        {
            var id = GetNumberAnswer("Удаление записи, введите идентификатор записи", int.MaxValue);
            if (id == -1) return;
            var record = logic.Get(id);
            if (record == null)
            {
                Console.WriteLine("По переданному идентификатору запись не найдена");
                return;
            }
            var ans = GetNumberAnswer($"Вы точно желаете удалить эту запись? 1.Да  2.Нет\n{record}", 2);
            if (ans == -1) return;
            if (ans == 2) return;
            logic.Delete(id);
        }

        /// <summary>
        /// Метод который отображает message читает консольный ввод и парсти в int 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="maxNum"></param>
        /// <returns></returns>
        static int GetNumberAnswer(string message, int maxNum)
        {
            Console.WriteLine(message);

            var ans = Console.ReadLine();

            if(int.TryParse(ans, out int result) && maxNum >= result && result > 0)
            {
                return result;
            }
            else
            {
                Console.WriteLine("Неизвестная команда");
                return -1;
            }
        }
    }
}
