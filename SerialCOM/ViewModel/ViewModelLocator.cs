using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;

namespace Kogler.SerialCOM
{
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            MefService.ComposeParts(this);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
        }

        private IEnumerable<ViewModelBase> _viewModels;
        [ImportMany(typeof(ViewModelBase))]
        public IEnumerable<ViewModelBase> ViewModels
        {
            get { return _viewModels; }
            set
            {
                _viewModels = value;
                var checkMethod = typeof(SimpleIoc).GetMethod("IsRegistered", Type.EmptyTypes);
                var registerMethod = typeof (SimpleIoc).GetMethods()
                    .Single(m =>
                        m.Name == "Register" &&
                        m.ContainsGenericParameters &&
                        m.GetParameters().Length == 0 &&
                        m.GetGenericArguments().Length == 1);
                foreach (var vm in value)
                {
                    var type = vm.GetType();
                    var genericCheckMethod = checkMethod.MakeGenericMethod(type);
                    if ((bool)genericCheckMethod.Invoke(SimpleIoc.Default, null)) continue;
                    registerMethod.MakeGenericMethod(type).Invoke(SimpleIoc.Default, null);
                    var name = type.Name;
                    if (name.EndsWith("ViewModel")) name = name.Substring(0, name.IndexOf("ViewModel", StringComparison.InvariantCultureIgnoreCase));
                    Dic.Add(name, vm);
                }
            }
        } 

        public ObservableDictionary<string, ViewModelBase> Dic { get; } = new ObservableDictionary<string, ViewModelBase>();
        
        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<MainViewModel>();
        }
    }
}