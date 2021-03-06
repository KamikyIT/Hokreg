﻿namespace Uniso.InStat.Classes
{
    public enum ActionEnum : int
    {
        //00 000 00
        _01_01 = 01 * 100000 + 01 * 100,    //Вбрасывание
        _01_02 = 01 * 100000 + 02 * 100,    //Передача

        _01_03 = 01 * 100000 + 03 * 100, //ПП
        _01_03_01 = 01 * 100000 + 03 * 100 + 01, //
        _01_03_02 = 01 * 100000 + 03 * 100 + 02, //

        _01_04 = 01 * 100000 + 04 * 100, //
        _01_04_01 = 01 * 100000 + 04 * 100 + 01, //
        _01_04_02 = 01 * 100000 + 04 * 100 + 02, //

        _01_05 = 01 * 100000 + 05 * 100, //
        _01_05_01 = 01 * 100000 + 05 * 100 + 01, //
        _01_05_02 = 01 * 100000 + 05 * 100 + 02, //

        _01_06_01 = 01 * 100000 + 06 * 100 + 01, //
        _01_06_02 = 01 * 100000 + 06 * 100 + 02, //

        _01_07 = 01 * 100000 + 07 * 100, //
        _01_07_01 = 01 * 100000 + 07 * 100 + 01, //
        _01_07_02 = 01 * 100000 + 07 * 100 + 02, //

        _01_08 = 01 * 100000 + 08 * 100, //
        _01_08_01 = 01 * 100000 + 08 * 100 + 01, //
        _01_08_02 = 01 * 100000 + 08 * 100 + 02, //

        _01_09 = 01 * 100000 + 09 * 100, // Отбивание шайбы вратарем
        _01_09_01 = 01 * 100000 + 09 * 100 + 01, //
        _01_09_02 = 01 * 100000 + 09 * 100 + 02, //

        _01_10 = 01 * 100000 + 10 * 100, // Пас в оффсайд

        _01_11 = 01 * 100000 + 11 * 100, // Отброс шайбы

        _02_01 = 02 * 100000 + 01 * 100, //
        _02_02_01 = 02 * 100000 + 02 * 100 + 1, //
        _02_02_02 = 02 * 100000 + 02 * 100 + 2, //
        _02_03 = 02 * 100000 + 03 * 100, //Ведение
        _02_03_01 = 02 * 100000 + 03 * 100 + 1, //Ведение
        _02_03_02 = 02 * 100000 + 03 * 100 + 2, //Ведение
        _02_04 = 02 * 100000 + 04 * 100, //
        _02_05 = 02 * 100000 + 05 * 100, //
        _02_06 = 02 * 100000 + 06 * 100, //
        _02_07 = 02 * 100000 + 07 * 100, //
        _02_07_02 = 02 * 100000 + 07 * 100 + 2, //
        _02_08 = 02 * 100000 + 08 * 100, //
        _02_09 = 02 * 100000 + 09 * 100, //
        _02_10 = 02 * 100000 + 10 * 100, //
        _02_11_01 = 02 * 100000 + 11 * 100 + 1, //Борьба у борта +
        _02_11_02 = 02 * 100000 + 11 * 100 + 2, //Борьба у борта -

        _12_01 = 12 * 100000 + 01 * 100, //
        _12_10 = 12 * 100000 + 10 * 100, //
        _12_11 = 12 * 100000 + 11 * 100, //
        _12_12 = 12 * 100000 + 12 * 100, //
        _12_13 = 12 * 100000 + 13 * 100, //
        _12_20 = 12 * 100000 + 20 * 100, //
        _12_21 = 12 * 100000 + 21 * 100, //
        _12_22 = 12 * 100000 + 22 * 100, //
        _12_23 = 12 * 100000 + 23 * 100, //
        _12_30 = 12 * 100000 + 30 * 100, //
        _12_31 = 12 * 100000 + 31 * 100, //
        _12_32 = 12 * 100000 + 32 * 100, //
        _12_33 = 12 * 100000 + 33 * 100, //


        _03_01 = 03 * 100000 + 01 * 100,    //Фол
        _03_02 = 03 * 100000 + 02 * 100,    //Фол отложенный
        _03_03 = 03 * 100000 + 03 * 100,    //Проброс
        _03_04 = 03 * 100000 + 04 * 100,    //Вне игры
        _03_05 = 03 * 100000 + 05 * 100,    //Шайба за пределами площадки
        _03_06 = 03 * 100000 + 06 * 100,    //Фиксация шайбы вратарем
        _03_07 = 03 * 100000 + 07 * 100,    //Фиксация шайбы игроком
        _03_08 = 03 * 100000 + 08 * 100,    //Стоп-игра (по не спортивным причинам)
        _03_09 = 03 * 100000 + 09 * 100,    //Офсайд

        _09_01 = 09 * 100000 + 01 * 100,    //Прочее
        _09_02 = 09 * 100000 + 02 * 100,    //Толчок соперника на борт
        _09_03 = 09 * 100000 + 03 * 100,    //Неправильная атака
        _09_04 = 09 * 100000 + 04 * 100,    //Грубость
        _09_05 = 09 * 100000 + 05 * 100,    //Игра высоко поднятой клюшкой
        _09_06 = 09 * 100000 + 06 * 100,    //Задержка руками/клюшкой
        _09_07 = 09 * 100000 + 07 * 100,    //Атака Игрока не владеющего шайбой (блокировка)
        _09_08 = 09 * 100000 + 08 * 100,    //Нарушение численного состава   
        _09_09 = 09 * 100000 + 09 * 100,    //Удар (клюшкой, локтем ногой головой)
        _09_10 = 09 * 100000 + 10 * 100,    //Подножка
        _09_11 = 09 * 100000 + 11 * 100,    //Сдвиг ворот
        _09_12 = 09 * 100000 + 12 * 100,    //Неспортивное поведение
        _09_13 = 09 * 100000 + 13 * 100,    //Неспортивное поведение
        _09_14 = 09 * 100000 + 14 * 100,    //Выброс шайбы

        _05_01 = 05 * 100000 + 01 * 100,    //ОКОНЧАНИЕ ШТРАФА
        _05_02 = 05 * 100000 + 02 * 100,    //МАЛЫЙ ШТРАФ (2`)
        _05_03 = 05 * 100000 + 03 * 100,    //МАЛЫЙ СКАМЕЕЧНЫЙ ШТРАФ (2`)
        _05_04 = 05 * 100000 + 04 * 100,    //ДВОЙНОЙ МАЛЫЙ ШТРАФ (4`)
        _05_05 = 05 * 100000 + 05 * 100,    //БОЛЬШОЙ ШТРАФ (5`)
        _05_06 = 05 * 100000 + 06 * 100,    //ДИСЦИПЛИНАРНЫЙ ШТРАФ (10`)
        _05_07 = 05 * 100000 + 07 * 100,    //ДИСЦИПЛИНАРНЫЙ ДО КОНЦА ИГРЫ ШТРАФ (ДКИ)
        _05_08 = 05 * 100000 + 08 * 100,    //МАТЧ-ШТРАФ (МШ)
        _05_09 = 05 * 100000 + 09 * 100,    //ШТРАФНОЙ БРОСОК (ШБ)

        _04_01 = 04 * 100000 + 01 * 100,    //Бросок мимо
        _04_02 = 04 * 100000 + 02 * 100,    //Бросок отбитый
        _04_03 = 04 * 100000 + 03 * 100,    //Бросок сблокированный
        _04_04 = 04 * 100000 + 04 * 100,    //Бросок в штангу
        _04_05 = 04 * 100000 + 05 * 100,    //Бросок зафиксированный
        _04_06 = 04 * 100000 + 06 * 100,    //Буллит

        _04_10 = 04 * 100000 + 10 * 100,    //
        _04_11 = 04 * 100000 + 11 * 100,    //

        _07_01 = 7 * 100000 + 01 * 100,    //

        _13_01 = 13 * 100000 + 01 * 100,    //
        _13_02 = 13 * 100000 + 02 * 100,    //

        _11_01_01 = 11 * 100000 + 01 * 100 + 1,    //
        _11_01_02 = 11 * 100000 + 01 * 100 + 2,    //
        _11_02_01 = 11 * 100000 + 02 * 100 + 1,    //
        _11_02_02 = 11 * 100000 + 02 * 100 + 2,    //
        _11_02_03 = 11 * 100000 + 02 * 100 + 3,    //
        _11_02_04 = 11 * 100000 + 02 * 100 + 4,    //
        _11_02_05 = 11 * 100000 + 02 * 100 + 5,    //
        _11_03_01 = 11 * 100000 + 03 * 100 + 1,    //
        _11_03_02 = 11 * 100000 + 03 * 100 + 2,    //
        _11_03_03 = 11 * 100000 + 03 * 100 + 3,    //
        _11_04_01 = 11 * 100000 + 04 * 100 + 1,    //

        _06_01 = 06 * 100000 + 01 * 100,    //Силовой прием
        _06_02 = 06 * 100000 + 02 * 100,    //Драка

        _08_01 = 08 * 100000 + 01 * 100,    //Гол
        _08_02 = 08 * 100000 + 02 * 100,    //Игроки забившей команды (+)
        _08_03 = 08 * 100000 + 03 * 100,    //Игроки пропустившей команды (-)

        //_14_01 = 14 * 100000 + 01 * 100,    //Замена

        _14_21 = 14 * 100000 + 21 * 100,    //ГК
        _14_12 = 14 * 100000 + 12 * 100,    //З Л
        _14_32 = 14 * 100000 + 32 * 100,    //З П
        _14_13 = 14 * 100000 + 13 * 100,    //Ф Л
        _14_23 = 14 * 100000 + 23 * 100,    //Ф Ц
        _14_33 = 14 * 100000 + 33 * 100,    //Ф П

        _16_121 = 16 * 100000 + 121 * 100,    //ГК
        _16_112 = 16 * 100000 + 112 * 100,    //З Л
        _16_132 = 16 * 100000 + 132 * 100,    //З П
        _16_113 = 16 * 100000 + 113 * 100,    //Ф Л
        _16_123 = 16 * 100000 + 123 * 100,    //Ф Ц
        _16_133 = 16 * 100000 + 133 * 100,    //Ф П

        _16_221 = 16 * 100000 + 221 * 100,    //ГК
        _16_212 = 16 * 100000 + 212 * 100,    //З Л
        _16_232 = 16 * 100000 + 232 * 100,    //З П
        _16_213 = 16 * 100000 + 213 * 100,    //Ф Л
        _16_223 = 16 * 100000 + 223 * 100,    //Ф Ц
        _16_233 = 16 * 100000 + 233 * 100,    //Ф П

        _16_321 = 16 * 100000 + 321 * 100,    //ГК
        _16_312 = 16 * 100000 + 312 * 100,    //З Л
        _16_332 = 16 * 100000 + 332 * 100,    //З П
        _16_313 = 16 * 100000 + 313 * 100,    //Ф Л
        _16_323 = 16 * 100000 + 323 * 100,    //Ф Ц
        _16_333 = 16 * 100000 + 333 * 100,    //Ф П

        _16_421 = 16 * 100000 + 421 * 100,    //ГК
        _16_412 = 16 * 100000 + 412 * 100,    //З Л
        _16_432 = 16 * 100000 + 432 * 100,    //З П
        _16_413 = 16 * 100000 + 413 * 100,    //Ф Л
        _16_423 = 16 * 100000 + 423 * 100,    //Ф Ц
        _16_433 = 16 * 100000 + 433 * 100,    //Ф П

        _16_521 = 16 * 100000 + 521 * 100,    //ГК
        _16_512 = 16 * 100000 + 512 * 100,    //З Л
        _16_532 = 16 * 100000 + 532 * 100,    //З П
        _16_513 = 16 * 100000 + 513 * 100,    //Ф Л
        _16_523 = 16 * 100000 + 523 * 100,    //Ф Ц
        _16_533 = 16 * 100000 + 533 * 100,    //Ф П

        _18_01 = 18 * 100000 + 01 * 100,    //Начало 1-го периода
        _18_02 = 18 * 100000 + 02 * 100,    //Начало 2-го периода
        _18_08 = 18 * 100000 + 08 * 100,    //Начало 3-го периода
        _18_05 = 18 * 100000 + 05 * 100,    //Начало овертайма
        _18_07 = 18 * 100000 + 07 * 100,    //Буллиты
        _18_04 = 18 * 100000 + 04 * 100,    //Конец матча
        _18_03 = 18 * 100000 + 03 * 100,    //Перерыв
        _18_06 = 18 * 100000 + 06 * 100,    //Перерыв на 30 секунд
        _18_09 = 18 * 100000 + 09 * 100,    //Коррекция времени по табло

        _10_01 = 10 * 100000 + 1 * 100,  //Потеря шайбы
        _10_02 = 10 * 100000 + 2 * 100,  //Овладевание шайбой
        _10_03 = 10 * 100000 + 3 * 100,  //Вход в зону атаки
        _10_04 = 10 * 100000 + 4 * 100,  //Выход из зоны обороны
    }
}
