using System;
using System.Collections.Generic;
using System.IO;
using kyoseki.Game.Serial;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace kyoseki.Game.Configuration
{
    public class KyosekiConfigManager : ConfigManager<KyosekiSetting>
    {
        protected virtual string Filename => "kyoseki.json";

        private readonly Storage storage;

        public KyosekiConfigManager(Storage storage, IDictionary<KyosekiSetting, object> defaultOverrides = null)
            : base(defaultOverrides)
        {
            this.storage = storage;

            InitialiseDefaults();
            Load();
        }

        protected override void InitialiseDefaults()
        {
            Set(KyosekiSetting.Skeletons, Array.Empty<SkeletonSerialProcessorInfo>());
        }

        protected override void PerformLoad()
        {
            using (var stream = storage.GetStream(Filename))
            {
                if (stream == null)
                    return;

                using (var reader = new StreamReader(stream))
                {
                    var config = reader.ReadToEnd();

                    var output = JsonConvert.DeserializeObject<Dictionary<KyosekiSetting, object>>(config);

                    foreach (var obj in output)
                    {
                        switch (obj.Key)
                        {
                            case KyosekiSetting.Skeletons:
                                var skeletons = JsonConvert.DeserializeObject<SkeletonSerialProcessorInfo[]>(obj.Value.ToString());

                                Set(KyosekiSetting.Skeletons, skeletons);
                                break;

                            default:
                                if (ConfigStore.TryGetValue(obj.Key, out IBindable b))
                                {
                                    try
                                    {
                                        b.Parse(obj.Value);
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Log($"Failed to parse config key {obj.Key}: {e}", LoggingTarget.Runtime, LogLevel.Important);
                                    }
                                }
                                else if (AddMissingEntries)
                                {
                                    Set(obj.Key, obj.Value);
                                }

                                break;
                        }
                    }
                }
            }
        }

        protected override bool PerformSave()
        {
            try
            {
                using (var stream = storage.GetStream(Filename, FileAccess.Write, FileMode.Create))
                using (var w = new StreamWriter(stream))
                {
                    var output = new Dictionary<KyosekiSetting, object>();

                    foreach (var p in ConfigStore)
                        output.Add(p.Key, p.Value);

                    w.Write(JsonConvert.SerializeObject(output));
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    public enum KyosekiSetting
    {
        Skeletons
    }
}