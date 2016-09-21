using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Uniso.InStat.Conv;

namespace Uniso.InStat.Classes
{
    [Serializable()]
    public class Options
    {
        public Options()
        {
            Hotkey_PauseResume = System.Windows.Forms.Keys.Q;
            Hotkey_Action_DumpIn = System.Windows.Forms.Keys.Space;
            Hotkey_RegisterBeginFix = System.Windows.Forms.Keys.F2;
            Hotkey_RegisterBeginNoFix = System.Windows.Forms.Keys.F3;
            Hotkey_StepPrev = System.Windows.Forms.Keys.F4;
            Hotkey_StepNext = System.Windows.Forms.Keys.F5;

            StepNextValue = 200;
            StepPrevValue = 1000;

            AutoSavePeriod = 0;

            Game_LengthPrimaryHalf = 20;
            Game_FindActualTimeForHalfFinally = true;
            Game_ShowActualTime = true;
        }

        #region Поля

        [Category("Горячие клавиши")]
        [DisplayName("1.Регистрация с фиксацией")]
        public Keys Hotkey_RegisterBeginFix { get; set; }

        [Category("Горячие клавиши")]
        [DisplayName("2.Регистрация без фиксации")]
        public Keys Hotkey_RegisterBeginNoFix { get; set; }

        [Category("Горячие клавиши")]
        [DisplayName("3.Шаг вперед")]
        public Keys Hotkey_StepNext { get; set; }//

        [Category("Горячие клавиши")]
        [DisplayName("4.Шаг назад")]
        public Keys Hotkey_StepPrev { get; set; }

        [Category("Горячие клавиши")]
        [DisplayName("5.Вбрасывание")]
        public Keys Hotkey_Action_DumpIn { get; set; }

        [Category("Горячие клавиши")]
        [DisplayName("6.Плей/Пауза")]
        public Keys Hotkey_PauseResume { get; set; }

        [Category("Действия")]
        [DisplayName("1.Вбрасывание")]
        public Keys HotKeyAction_1_1 { get; set; }

        [Category("Действия")]
        [DisplayName("2.Пас по борту")]
        public Keys HotKeyAction_1_3 { get; set; }

        [Category("Действия")]
        [DisplayName("3.КП")]
        public Keys HotKeyAction_1_4 { get; set; }

        [Category("Действия")]
        [DisplayName("4.ОП")]
        public Keys HotKeyAction_1_5 { get; set; }

        [Category("Действия")]
        [DisplayName("5.Выброс (+)")]
        public Keys HotKeyAction_1_6_1 { get; set; }

        [Category("Действия")]
        [DisplayName("6.Выброс (-)")]
        public Keys HotKeyAction_1_6_2 { get; set; }

        [Category("Действия")]
        [DisplayName("7.Вброс")]
        public Keys HotKeyAction_1_8 { get; set; }

        [Category("Действия")]
        [DisplayName("8.Единоборство")]
        public Keys HotKeyAction_2_1 { get; set; }

        [Category("Действия")]
        [DisplayName("9.Обводка (+)")]
        public Keys HotKeyAction_2_2_1 { get; set; }

        [Category("Действия")]
        [DisplayName("10.Обводка (-)")]
        public Keys HotKeyAction_2_2_2 { get; set; }

        [Category("Действия")]
        [DisplayName("11.Ведение")]
        public Keys HotKeyAction_2_3 { get; set; }

        [Category("Действия")]
        [DisplayName("12.Прием/Подбор")]
        public Keys HotKeyAction_2_4 { get; set; }

        [Category("Действия")]
        [DisplayName("13.Отбор")]
        public Keys HotKeyAction_2_6 { get; set; }

        [Category("Действия")]
        [DisplayName("14.Перехват")]
        public Keys HotKeyAction_2_7 { get; set; }

        [Category("Действия")]
        [DisplayName("15.Перехват неудачный")]
        public Keys HotKeyAction_2_7_2 { get; set; }

        [Category("Действия")]
        [DisplayName("16.Неудачная обработка")]
        public Keys HotKeyAction_2_8 { get; set; }

        [Category("Действия")]
        [DisplayName("17.Помеха")]
        public Keys HotKeyAction_2_9 { get; set; }

        [Category("Действия")]
        [DisplayName("18.Вбр/Стоп-игра")]
        public Keys HotKey_1_1_1_2 { get; set; }

        [Category("Действия")]
        [DisplayName("19.Силовой прием")]
        public Keys HotKeyAction_6_1 { get; set; }

        //--------------------------

        [Category("Видео")]
        [DisplayName("1.Шаг вперед, мс")]
        public int StepNextValue { get; set; }

        [Category("Видео")]
        [DisplayName("2.Шаг назад, мс")]
        public int StepPrevValue { get; set; }

        [Category("Видео")]
        [DisplayName("3.Останавливать на бросках")]
        [TypeConverter(typeof(BoolConverter))]
        public bool IsStopPlayingOnShot { get; set; }

        //--------------------------

        [Category("Сервер")]
        [DisplayName("1.Период автосохранения")]
        [Description("Период задается в секундах. При 0 - автосохранение не производится")]
        public int AutoSavePeriod { get; set; }

        [Category("Игра")]
        [DisplayName("1.Длительность основных периодов, мин")]
        public int Game_LengthPrimaryHalf { get; set; }

        [Category("Игра")]
        [DisplayName("2.Подбирать время окончания периодов")]
        [TypeConverter(typeof(BoolConverter))]
        public bool Game_FindActualTimeForHalfFinally { get; set; }

        [Category("Игра")]
        [DisplayName("3.Показывать чистое время")]
        [TypeConverter(typeof(BoolConverter))]
        public bool Game_ShowActualTime { get; set; }

        [Category("Игра")]
        [DisplayName("4.Не останавливать видео в онлайн режиме")]
        [TypeConverter(typeof(BoolConverter))]
        public bool Game_NoStopVideoInOnline { get; set; }

        [Category("Игра")]
        [DisplayName("5.Разрешить сохранять аварийные маркеры")]
        [TypeConverter(typeof(BoolConverter))]
        public bool Game_EnableFailureSaving { get; set; }

        [Category("Игра")]
        [DisplayName("6.Разрешить подтверждать время маркеров с временем")]
        [TypeConverter(typeof(BoolConverter))]
        public bool Game_EnableApplyTimeEx { get; set; }

        #endregion
        
        #region FILE WORKING

        private static Options options = null;

        private const string options_data_file = @".\\.options.dat";

        public static Options G
        {
            get
            {
                if (options == null)
                    options = Options.Load();

                if (options == null)
                {
                    options = new Options();
                    options.Save();
                }

                return options;
            }
        }

        private static String _optionsFileName = String.Empty;
        private static String optionsFileName {
            get
            {
                if (_optionsFileName == string.Empty)
                {
                    var fi = new FileInfo(Application.ExecutablePath);
                    // TODO: Перепилить Path.
                    _optionsFileName = fi.Directory + options_data_file;
                }
                return _optionsFileName;
            }
            set { _optionsFileName = value; }
        }

        [Obsolete("Use optionsFileName.")]
        private static String GetOptionsFileName()
        {
            if (optionsFileName == String.Empty)
            {
                var fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
                optionsFileName = fi.Directory + @".\\.options.dat";
            }

            return optionsFileName;
        }

        public void Save()
        {
            Stream serializationStream = new FileStream(optionsFileName, FileMode.Create, FileAccess.Write);
            try
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(serializationStream, this);
            }
            catch
            { }
            finally
            {
                serializationStream.Close();
            }
        }

        public static Options Load()
        {
            if (!File.Exists(optionsFileName))
            {
                return null;
            }

            using (Stream serializationStream = new FileStream(optionsFileName, FileMode.Open, FileAccess.Read))
            {
                var formatter = new BinaryFormatter();
                var res = (Options)formatter.Deserialize(serializationStream);

                if (res.Game_LengthPrimaryHalf == 0)
                    res.Game_LengthPrimaryHalf = 20;

                return res;
            }
        }

        #endregion
    }
}
