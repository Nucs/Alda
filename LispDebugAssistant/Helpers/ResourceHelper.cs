using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using autonet.lsp;

namespace Nucs.Alda.Helpers {
    public static class ResourceHelper {
        /// <summary>
        ///     Searches for a resource and exports it to a file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static LspLoader.ResourceInfo GetResource(string name, int version = -1) {
            string ReplaceCaseInsensitive(string input, string search, string replacement) {
                string result = Regex.Replace(
                    input,
                    Regex.Escape(search),
                    replacement.Replace("$", "$$"),
                    RegexOptions.IgnoreCase
                );
                return result;
            }

            name = Path.ChangeExtension(name, null) + (version == -1 ? "" : $".{version}");
            var asms = AppDomain.CurrentDomain.GetAssemblies().Union(new[] {Assembly.GetCallingAssembly()}).Distinct();
            LspLoader.Resource target = null;
            var targets = asms
                .SelectMany(asm => {
                    try {
                        return asm.GetManifestResourceNames().Select(rs => new LspLoader.Resource(asm, rs));
                    } catch (NotSupportedException) { }
                    return new LspLoader.Resource[0];
                })
                .Where(res => res.Contains(name))
                .ToArray();

            if (targets.Length == 0)
                throw new ResourceNotFoundException($"Could not find a resource that contains the name '{name}'");
            if (targets.Length > 1) {
                //resolve versions.
                if (targets.All(t => Path.ChangeExtension(t.ResourceName, null).Contains('.')) == false)
                    throw new ResourceNotFoundException($"Could not resolve resource: '{name}'\nVersus:\n{string.Join("\n", targets.Select(t => t.ResourceName))}");
                target = targets.OrderByDescending(t => {
                        var _i = Path.ChangeExtension(t.ResourceName, null).Split('.').Last();
                        if (_i.All(char.IsDigit) == false)
                            throw new ResourceNotFoundException($"Could not resolve resource: '{name}'\nVersus:\n{string.Join("\n", targets.Select(tt => tt.ResourceName))}");
                        return int.Parse(_i);
                    })
                    .FirstOrDefault();
            } else if (targets.Length == 1) {
                target = targets[0];
            }
           
            var ret = new LspLoader.ResourceInfo() {FileName = target.ResourceName, Content = target.ReadResource()};
            return ret;
        }
    }
}