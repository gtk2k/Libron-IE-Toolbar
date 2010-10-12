using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;

namespace LibronToolbar
{
    [RunInstaller(true)]
    public partial class InstallerClass : Installer
    {
        public InstallerClass()
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
                throw new InstallException("Libron�c�[���o�[�̓o�^�Ɏ��s���܂���");
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.UnregisterAssembly(this.GetType().Assembly))
            {
                throw new InstallException("libron�c�[���o�[�̓o�^�����Ɏ��s���܂���");
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