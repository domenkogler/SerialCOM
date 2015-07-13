namespace Kogler.SerialCOM
{
    public class Export
    {
        public class PortsControlExport : AnchorableViewModel
        {
            public PortsControlExport()
            {
                Title = "Ports";
                Content = new PortsControl {DataContext = this};
            }
        }
    }
}
