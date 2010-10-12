using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;


namespace LibronToolbarInstaller
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.RegisterAssembly(this.GetType().Assembly,
            AssemblyRegistrationFlags.SetCodeBase))
            {
                throw new InstallException("Libronツールバーの登録に失敗しました");
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.UnregisterAssembly(this.GetType().Assembly))
            {
                throw new InstallException("libronツールバーの登録解除に失敗しました");
            }
        }

        private void InstallerClass_AfterInstall(object sender, InstallEventArgs e)
        {
            //string explorerPath = @"C:\Program Files\Internet Explorer\iexplore.exe";
            //if (System.IO.File.Exists(explorerPath) == true)
            //{
            //    System.Diagnostics.Process.Start(explorerPath);
            //}
        }
    }
}
