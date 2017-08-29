﻿using System;
using System.IO;
using System.Runtime.Serialization;
using Common;
using Newtonsoft.Json;

namespace autonet {
    namespace Common.Settings {
        public abstract class JsonConfiguration : ISaveable, ICloneable {
            private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings {Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Include};
            private readonly Type _childtype;

            protected JsonConfiguration() {
                _childtype = GetType();
            }

            [JsonIgnore]
            public abstract string FileName { get; }

            /// <summary>
            ///     The filename that was originally loaded from. saving to other file does not change this field!
            /// </summary>
            /// <param name="filename">the name of the file, ##DEFAULT## is the default.</param>
            public virtual void Save(string filename) {
                Save(_childtype, this, filename);
                //File.WriteAllText(filename, JsonConvert.SerializeObject(this));
            }

            /// <summary>
            ///     Save the settings file to a predefined location <see cref="ISaveable.FileName" />
            /// </summary>
            public void Save() {
                Save("##DEFAULT##");
            }

            /// <summary>
            ///     Invoked after loading to this new object. <br></br>
            ///     Can be used to validate the inputted data.<br></br>
            ///     Incase of a new instance, AfterLoad first and then saving.
            /// </summary>
            public virtual void AfterLoad() { }

            /// <summary>
            ///     Invoked before saving this object.
            /// </summary>
            public virtual void BeforeSave() { }

            /// <summary>
            ///     Invoked after saving this object.
            /// </summary>
            public virtual void AfterSave() { }

            /// <summary>
            ///     Saves settings to a given path.
            /// </summary>
            public static void Save(Type intype, object pSettings, string filename = "##DEFAULT##") {
                if (pSettings is ISaveable == false)
                    throw new ArgumentException("Given param is not ISavable!", nameof(pSettings));
                var o = (ISaveable) pSettings;
                if (filename == "##DEFAULT##")
                    filename = (string) intype.GetProperty("FileName", typeof(string)).GetMethod.Invoke(o, null);

                if (filename.Contains("/") || filename.Contains("\\")) {
                    filename = Path.Combine(Paths.NormalizePath(Path.GetDirectoryName(filename)), Path.GetFileName(filename));
                    if (Directory.Exists(Path.GetDirectoryName(filename)) == false)
                        Directory.CreateDirectory(Path.GetDirectoryName(filename));
                } else {
                    filename = Paths.CombineToExecutingBase(filename).FullName;
                }
                lock (o) {
                    o.BeforeSave();
                    File.WriteAllText(filename, JsonConvert.SerializeObject(o, intype, _settings));
                    o.AfterSave();
                }
            }

            /// <summary>
            ///     Loads or creates a settings file.
            /// </summary>
            /// <param name="intype">The type of this object</param>
            /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
            /// <param name="preventoverride">If the file did not exist or corrupt, dont resave it</param>
            /// <returns>The loaded or freshly new saved object</returns>
            public static object Load(Type intype, string filename = "##DEFAULT##") {
                JsonConfiguration fresh = null;
                if (filename == "##DEFAULT##")
                    filename = (fresh = (JsonConfiguration) Activator.CreateInstance(intype)).FileName;

                if (filename.Contains("/") || filename.Contains("\\"))
                    filename = Path.Combine(Paths.NormalizePath(Path.GetDirectoryName(filename)), Path.GetFileName(filename));

                if (File.Exists(filename))
                    try {
                        var fc = File.ReadAllText(filename);
                        if (string.IsNullOrEmpty((fc ?? "").Replace("\r", "").Replace("\n", "").Trim()))
                            throw new SettingsException("The settings file is empty!");
                        var o = (ISaveable) JsonConvert.DeserializeObject(fc, intype, _settings);
                        o.AfterLoad();
                        return o;
                    }
                    catch (InvalidOperationException e) when (e.Message.Contains("Cannot convert")) {
                        throw new SettingsException("Unable to deserialize settings file, value<->type mismatch. see inner exception", e);
                    }
                    catch (ArgumentException e) when (e.Message.StartsWith("Invalid")) {
                        throw new SettingsException("Settings file is corrupt.");
                    }

                //doesnt exist.
                fresh = fresh ?? (JsonConfiguration) Activator.CreateInstance(intype);
                fresh.AfterLoad();
                fresh.Save(filename);

                return fresh;
            }

            /// <summary>
            ///     Saves settings to a given path.
            /// </summary>
            public static void Save<T>(T pSettings, string filename = "##DEFAULT##") where T : ISaveable {
                Save(typeof(T), pSettings, filename);
            }

            /// <summary>
            ///     Loads or creates a settings file.
            /// </summary>
            /// <param name="filename">File name, for example "settings.jsn". no path required, just a file name.</param>
            /// <param name="preventoverride">If the file did not exist or corrupt, dont resave it</param>
            /// <returns>The loaded or freshly new saved object</returns>
            public static T Load<T>(string filename = "##DEFAULT##") where T : ISaveable, new() {
                return (T) Load(typeof(T), filename);
            }

            /*        /// <summary>
            ///     Gives you all the types that has
            ///     <param name="attribute"></param>
            ///     attached to it in the entire <see cref="AppDomain" />.
            /// </summary>
            /// <param name="attribute">the type of the attribute.</param>
            /// <returns></returns>
            private static IEnumerable<Type> GetAllAttributeHolders(Type attribute) {
                return from assmb in AppDomain.CurrentDomain.GetAssemblies() from type in gettypes(assmb) where type.GetCustomAttributes(attribute, true).Length > 0 select type;
            }

            private static Type[] gettypes(Assembly assmb) {
                return !File.Exists(AssemblyDirectory(assmb)) ? new Type[0] : assmb.GetTypes();
            }

            private static string AssemblyDirectory(Assembly asm) {
                var codeBase = asm.CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
    #pragma warning disable 693
            private static T CreateInstance<T>(Type @this) {
                return (T) Activator.CreateInstance(@this);
            }*/
            public object Clone() {
                return this.Copy();
            }
        }

        [Serializable]
        public class SettingsException : Exception {
            public SettingsException() { }
            public SettingsException(string message) : base(message) { }
            public SettingsException(string message, Exception inner) : base(message, inner) { }

            protected SettingsException(
                SerializationInfo info,
                StreamingContext context) : base(info, context) { }
        }
        
    }
}