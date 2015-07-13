using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Kogler.SerialCOM
{
    public static class MefService
    {
        public static void Init()
        {
            Catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            Catalog.Catalogs.Add(new DirectoryCatalog("Plugins"));
        }

        public static AggregateCatalog Catalog { get; } = new AggregateCatalog();
        public static CompositionContainer Container { get; } = new CompositionContainer(Catalog);

        public static void ComposeParts(params object[] parts)
        {
            Container.ComposeParts(parts);
        }
    }
}