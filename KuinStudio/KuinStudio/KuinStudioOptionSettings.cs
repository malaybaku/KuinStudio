using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Baku.KuinStudio
{
    [DataContract]
    public class KuinStudioOptionSettings : INotifyPropertyChanged
    {
        private KuinStudioOptionSettings()
        {

        }

        [DataMember]
        private string _kuinCompilerPath = "";
        [IgnoreDataMember]
        public string KuinCompilerPath
        {
            get { return _kuinCompilerPath; }
            set { SetProperty(ref _kuinCompilerPath, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [IgnoreDataMember]
        private static KuinStudioOptionSettings _instance;
        [IgnoreDataMember]
        public static KuinStudioOptionSettings Instance
            => _instance ?? (_instance = LoadFromFile());

        [IgnoreDataMember]
        private static readonly string ConfigFileName = "KuinStudio.config.json";

        [IgnoreDataMember]
        private static string AssemblyLocation
            => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        [IgnoreDataMember]
        private static string ConfigFilePath
            => Path.Combine(string.IsNullOrEmpty(AssemblyLocation) ? "" : AssemblyLocation, ConfigFileName);

        private void SaveToFile()
        {
            using (var s = new StreamWriter(ConfigFilePath, false))
            {
                new DataContractJsonSerializer(typeof(KuinStudioOptionSettings))
                    .WriteObject(s.BaseStream, this);
            }
        }

        private static KuinStudioOptionSettings LoadFromFile()
        {
            try
            {
                using (var s = new StreamReader(ConfigFilePath))
                {
                    return new DataContractJsonSerializer(typeof(KuinStudioOptionSettings))
                        .ReadObject(s.BaseStream)
                        as KuinStudioOptionSettings;
                }
            }
            catch (Exception)
            {
                //初回あるいはエラー時リセット
                return new KuinStudioOptionSettings();
            }
        }

        private void SetProperty<T>(ref T target, T value, [CallerMemberName]string pname = "")
            where T : IEquatable<T>
        {
            if (!target.Equals(value))
            {
                target = value;
                //変更かかるたびに保存する: ファイル小さいし更新頻度も低いからこれでOKと判断
                SaveToFile();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pname));
            }
        }
    }
}
