using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Uniso.InStat.Game.Hockey
{
    public class ActionConverter : TypeConverter
    {
        Dictionary<ActionEnum, String> _v = new Dictionary<ActionEnum, string>();

        public ActionConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            GetValues(context);

            int i;
            if (value is string)
            {
                var _vv = _v.Values.ToArray<String>();
                var str = ((string)value).ToLower();
                for (i = 0; i < _v.Count; ++i)
                {
                    if (_vv[i].ToLower() == str)
                        return _v.Keys.ToArray<ActionEnum>()[i];
                }

                //throw new ArgumentException();
                return null;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            GetValues(context);

            int i;
            if ((destinationType == typeof(string)) && (_v.Count > 0))
            {
                if (value is ActionEnum)
                {
                    var _vv = _v.Keys.ToArray<ActionEnum>();
                    for (i = 0; i < _v.Keys.Count; ++i)
                    {
                        if (_vv[i].Equals(value))
                            return _v[_vv[i]];
                    }

                    try
                    {
                        var v2 = (ActionEnum)((int)value & 0x0000ffff);
                        for (i = 0; i < _v.Keys.Count; ++i)
                        {
                            if (_vv[i].Equals(v2))
                            {
                                //int win = ((int)value & 0x00ff0000) >> 16;
                                var ret = _v[_vv[i]];
                                /*if (win == 1)
                                    ret += " (+)";
                                if (win == 2)
                                    ret += " (-)";*/

                                return ret;
                            }
                        }
                    }
                    catch
                    { }

                    var v = (int)value;

                    var ActionId = v / 100000;
                    var ActionType = (v / 100) % 1000;
                    var Win = v % 100;

                    return String.Format("{0}-{1}-{2}", ActionId, ActionType, Win);
                }
                else
                    //throw new Exception();
                    return null;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            GetValues(context);

            return new StandardValuesCollection(_v.Keys);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public void GetValues(ITypeDescriptorContext context)
        {
            if (_v.Count == 0)
            {
                _v.Clear();

                _v.Add(ActionEnum._01_01, "Вбрасывание");
                _v.Add(ActionEnum._01_02, "Голевая передача");

                _v.Add(ActionEnum._01_03, "Пас по борту");
                _v.Add(ActionEnum._01_03_01, "Пас по борту (+)");
                _v.Add(ActionEnum._01_03_02, "Пас по борту (-)");

                _v.Add(ActionEnum._01_04, "КП");
                _v.Add(ActionEnum._01_04_01, "КП (+)");
                _v.Add(ActionEnum._01_04_02, "КП (-)");

                _v.Add(ActionEnum._01_05, "ОП");
                _v.Add(ActionEnum._01_05_01, "ОП (+)");
                _v.Add(ActionEnum._01_05_02, "ОП (-)");

                _v.Add(ActionEnum._01_06_01, "Выброс (+)");
                _v.Add(ActionEnum._01_06_02, "Выброс (-)");

                _v.Add(ActionEnum._01_08, "Вброс");
                _v.Add(ActionEnum._01_08_01, "Вброс (+)");
                _v.Add(ActionEnum._01_08_02, "Вброс (-)");

                _v.Add(ActionEnum._01_10, "Заброс безадресный");

                _v.Add(ActionEnum._02_01, "Единоборство");
                _v.Add(ActionEnum._02_02_01, "Обводка (+)");
                _v.Add(ActionEnum._02_02_02, "Обводка (-)");
                _v.Add(ActionEnum._02_03, "Ведение");
                _v.Add(ActionEnum._02_03_01, "Продолжение ведения");
                _v.Add(ActionEnum._02_03_02, "Ведение вдоль борта");
                _v.Add(ActionEnum._02_04, "Прием");
                _v.Add(ActionEnum._02_05, "Подбор");
                _v.Add(ActionEnum._02_06, "Отбор");
                _v.Add(ActionEnum._02_07, "Перехват");
                _v.Add(ActionEnum._02_07_02, "Перехват неудачный");
                _v.Add(ActionEnum._02_08, "Неудачная обработка");
                _v.Add(ActionEnum._02_09, "Помеха");
                _v.Add(ActionEnum._02_10, "Подбор в борьбе");
                _v.Add(ActionEnum._02_11_01, "Борьба у борта (+)");
                _v.Add(ActionEnum._02_11_02, "Борьба у борта (-)");

                _v.Add(ActionEnum._12_10, "Атака с ходу 1-0");
                _v.Add(ActionEnum._12_11, "Атака с ходу 1-1");
                _v.Add(ActionEnum._12_12, "Атака с ходу 1-2");
                _v.Add(ActionEnum._12_13, "Атака с ходу 1-3");
                _v.Add(ActionEnum._12_20, "Атака с ходу 2-0");
                _v.Add(ActionEnum._12_21, "Атака с ходу 2-1");
                _v.Add(ActionEnum._12_22, "Атака с ходу 2-2");
                _v.Add(ActionEnum._12_23, "Атака с ходу 2-3");
                _v.Add(ActionEnum._12_30, "Атака с ходу 3-0");
                _v.Add(ActionEnum._12_31, "Атака с ходу 3-1");
                _v.Add(ActionEnum._12_32, "Атака с ходу 3-2");
                _v.Add(ActionEnum._12_33, "Атака с ходу 3-3");
                _v.Add(ActionEnum._12_01, "Конец атаки с ходу");

                _v.Add(ActionEnum._03_01, "Фол");
                _v.Add(ActionEnum._03_02, "Фол отложенный");
                _v.Add(ActionEnum._03_03, "Проброс");
                _v.Add(ActionEnum._03_04, "Вне игры");
                _v.Add(ActionEnum._03_05, "Шайба за пределами площадки");
                _v.Add(ActionEnum._03_06, "Фиксация шайбы вратарем");
                _v.Add(ActionEnum._03_07, "Фиксация шайбы игроком");
                _v.Add(ActionEnum._03_08, "Стоп-игра");
                _v.Add(ActionEnum._03_09, "Офсайд");

                _v.Add(ActionEnum._09_01, "Прочее");
                _v.Add(ActionEnum._09_02, "Толчок соперника на борт");
                _v.Add(ActionEnum._09_03, "Неправильная атака");
                _v.Add(ActionEnum._09_04, "Грубость");
                _v.Add(ActionEnum._09_05, "Игра высоко поднятой клюшкой");
                _v.Add(ActionEnum._09_06, "Задержка руками/клюшкой");
                _v.Add(ActionEnum._09_07, "Атака Игрока, не владеющего шайбой (блокировка)");
                _v.Add(ActionEnum._09_08, "Нарушение численного состава");
                _v.Add(ActionEnum._09_09, "Удар (клюшкой, локтем, ногой головой)");
                _v.Add(ActionEnum._09_10, "Подножка");
                _v.Add(ActionEnum._09_11, "Сдвиг ворот");
                _v.Add(ActionEnum._09_12, "Неспортивное поведение");
                _v.Add(ActionEnum._09_13, "Драка");
                _v.Add(ActionEnum._09_14, "Выброс шайбы");

                
                _v.Add(ActionEnum._05_01, "ШТРАФ ЗАВЕРШЕН");
                _v.Add(ActionEnum._05_02, "МАЛЫЙ ШТРАФ (2`)");
                _v.Add(ActionEnum._05_03, "МАЛЫЙ СКАМЕЕЧНЫЙ ШТРАФ (2`)");
                _v.Add(ActionEnum._05_04, "ДВОЙНОЙ МАЛЫЙ ШТРАФ (4`)");
                _v.Add(ActionEnum._05_05, "БОЛЬШОЙ ШТРАФ (5`)");
                _v.Add(ActionEnum._05_06, "ДИСЦИПЛИНАРНЫЙ ШТРАФ (10`)");
                _v.Add(ActionEnum._05_07, "ДИСЦИПЛИНАРНЫЙ ДО КОНЦА ИГРЫ ШТРАФ (ДКИ)");
                _v.Add(ActionEnum._05_08, "МАТЧ-ШТРАФ (МШ)");
                _v.Add(ActionEnum._05_09, "ШТРАФНОЙ БРОСОК (ШБ)");

                _v.Add(ActionEnum._04_01, "Бросок мимо");
                _v.Add(ActionEnum._04_02, "Бросок отбитый");
                _v.Add(ActionEnum._04_03, "Бросок заблокированный");
                _v.Add(ActionEnum._04_04, "Бросок в штангу");
                _v.Add(ActionEnum._04_05, "Бросок зафиксированный");
                _v.Add(ActionEnum._04_06, "Буллит");
                _v.Add(ActionEnum._04_10, "Бросок-щелчок");
                _v.Add(ActionEnum._04_11, "Бросок кистевой");

                _v.Add(ActionEnum._06_01, "Силовой прием");

                _v.Add(ActionEnum._13_01, "Грубая ошибка");
                _v.Add(ActionEnum._13_02, "Грубая голевая ошибка");

                _v.Add(ActionEnum._07_01, "Сэйв");

                _v.Add(ActionEnum._08_01, "Гол");
                _v.Add(ActionEnum._08_02, "Игроки забившей команды (+)");
                _v.Add(ActionEnum._08_03, "Игроки пропустившей команды (-)");

                _v.Add(ActionEnum._14_21, "Замена ГК");
                _v.Add(ActionEnum._14_12, "Замена З Л");
                _v.Add(ActionEnum._14_32, "Замена З П");
                _v.Add(ActionEnum._14_13, "Замена Ф Л");
                _v.Add(ActionEnum._14_23, "Замена Ф Ц");
                _v.Add(ActionEnum._14_33, "Замена Ф П");

                _v.Add(ActionEnum._16_121, "ГК (1)");
                _v.Add(ActionEnum._16_112, "З Л (1)");
                _v.Add(ActionEnum._16_132, "З П (1)");
                _v.Add(ActionEnum._16_113, "Ф Л (1)");
                _v.Add(ActionEnum._16_123, "Ф Ц (1)");
                _v.Add(ActionEnum._16_133, "Ф П (1)");

                _v.Add(ActionEnum._16_221, "ГК (2)");
                _v.Add(ActionEnum._16_212, "З Л (2)");
                _v.Add(ActionEnum._16_232, "З П (2)");
                _v.Add(ActionEnum._16_213, "Ф Л (2)");
                _v.Add(ActionEnum._16_223, "Ф Ц (2)");
                _v.Add(ActionEnum._16_233, "Ф П (2)");

                _v.Add(ActionEnum._16_321, "ГК (3)");
                _v.Add(ActionEnum._16_312, "З Л (3)");
                _v.Add(ActionEnum._16_332, "З П (3)");
                _v.Add(ActionEnum._16_313, "Ф Л (3)");
                _v.Add(ActionEnum._16_323, "Ф Ц (3)");
                _v.Add(ActionEnum._16_333, "Ф П (3)");

                _v.Add(ActionEnum._16_421, "ГК (4)");
                _v.Add(ActionEnum._16_412, "З Л (4)");
                _v.Add(ActionEnum._16_432, "З П (4)");
                _v.Add(ActionEnum._16_413, "Ф Л (4)");
                _v.Add(ActionEnum._16_423, "Ф Ц (4)");
                _v.Add(ActionEnum._16_433, "Ф П (4)");

                _v.Add(ActionEnum._16_521, "ГК (5)");
                _v.Add(ActionEnum._16_512, "З Л (5)");
                _v.Add(ActionEnum._16_532, "З П (5)");
                _v.Add(ActionEnum._16_513, "Ф Л (5)");
                _v.Add(ActionEnum._16_523, "Ф Ц (5)");
                _v.Add(ActionEnum._16_533, "Ф П (5)");

                _v.Add(ActionEnum._18_01, "Начало 1-го периода");
                _v.Add(ActionEnum._18_02, "Начало 2-го периода");
                _v.Add(ActionEnum._18_08, "Начало 3-го периода");
                _v.Add(ActionEnum._18_05, "Начало овертайма");
                _v.Add(ActionEnum._18_07, "Буллиты");
                _v.Add(ActionEnum._18_04, "Конец матча");
                _v.Add(ActionEnum._18_03, "Перерыв");
                _v.Add(ActionEnum._18_06, "Перерыв на 30 секунд");
                _v.Add(ActionEnum._18_09, "Коррекция времени с онлайном");

                _v.Add(ActionEnum._11_01_01, "Бросок правой рукой");
                _v.Add(ActionEnum._11_01_02, "Бросок левой рукой");

                _v.Add(ActionEnum._11_02_01, "Относительно вратаря: ДОМИК");
                _v.Add(ActionEnum._11_02_02, "Относительно вратаря: под левой рукой");
                _v.Add(ActionEnum._11_02_03, "Относительно вратаря: под правой рукой");
                _v.Add(ActionEnum._11_02_04, "Относительно вратаря: над левой рукой");
                _v.Add(ActionEnum._11_02_05, "Относительно вратаря: над правой рукой");

                _v.Add(ActionEnum._11_03_01, "Позиция вратаря: СТОИТ");
                _v.Add(ActionEnum._11_03_02, "Позиция вратаря: СИДИТ");
                _v.Add(ActionEnum._11_03_03, "Позиция вратаря: ЛЕЖИТ");

                _v.Add(ActionEnum._11_04_01, "Точка в воротах");

                _v.Add(ActionEnum._10_01, "Потеря шайбы");
                _v.Add(ActionEnum._10_02, "Овладевание шайбой");
                _v.Add(ActionEnum._10_03, "Вход в зону атаки");
                _v.Add(ActionEnum._10_04, "Выход из зоны обороны");
            }
        }
    }
}
