using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SHDocVw;
using BandObjectLib;

namespace LibronToolbar
{
    /// <summary>
    /// Registration:
    /// This is a browser helper object, which is registered as a COM When we register the 
    /// SearchBar.dll using the regasm command.
    /// Loading:
    /// This COM object loaded for each IE window. As a window is created, it creates its own copy of the BHO; 
    /// and, when that window is closed, it destroys its copy of the BHO
    /// Purpose of implementing this BHO:
    /// It loads the toolbar when this BHO is instantiated.
    /// Code Reference: http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=509297&SiteID=1
    /// </summary>
   [ComVisible(true)]
   [Guid("1D970ED5-3EDA-438d-BFFD-715931E2775B")]
   [ClassInterface(ClassInterfaceType.None)]
   public class InitToolbarBHO : IObjectWithSite
   {
       #region Fields
       private InternetExplorer explorer;
       private const string BHOKeyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Browser Helper Objects";
       #endregion

       #region Com Register/UnRegister Methods
       /// <summary>
       /// Called, when IE browser starts.
       /// </summary>
       /// <param name="t"></param>
       [ComRegisterFunction]
       public static void RegisterBHO(Type t)
       {
           RegistryKey key = Registry.LocalMachine.OpenSubKey(BHOKeyName, true);
           if (key == null)
           {
               key = Registry.LocalMachine.CreateSubKey(BHOKeyName);
           }
           string guidString = t.GUID.ToString("B");
           RegistryKey bhoKey = key.OpenSubKey(guidString, true);

           if (bhoKey == null)
           {
               bhoKey = key.CreateSubKey(guidString);
           }

           // NoExplorer:dword = 1 prevents the BHO to be loaded by Explorer
           string _name = "NoExplorer";
           object _value = (object)1;
           bhoKey.SetValue(_name, _value);
           key.Close();
           bhoKey.Close();
       }

       /// <param name="t"></param>
       [ComUnregisterFunction]
       public static void UnregisterBHO(Type t)
       {
           RegistryKey key = Registry.LocalMachine.OpenSubKey(BHOKeyName, true);
           string guidString = t.GUID.ToString("B");
           if (key != null)
           {
               key.DeleteSubKey(guidString, false);
           }
       }
      #endregion

       #region IObjectWithSite Members
       /// <summary>
       /// Called, when the BHO is instantiated and when it is destroyed.
       /// </summary>
       /// <param name="site"></param>
       public void SetSite(Object site)
       {
           if (site != null)
           {
               explorer = (InternetExplorer)site;
               ShowBrowserBar(true);
           }
       }

       public void GetSite(ref Guid guid, out Object ppvSite)
       {
           IntPtr punk = Marshal.GetIUnknownForObject(explorer);
           ppvSite = new object();
           IntPtr ppvSiteIntPtr =  Marshal.GetIUnknownForObject(ppvSite);
           int hr = Marshal.QueryInterface(punk, ref guid, out ppvSiteIntPtr);
           Marshal.Release(punk);
       }
      #endregion

       #region Helper Methods
      private void ShowBrowserBar(bool bShow)
      {
          //GUID_OF_Your_Toolbar: C9A6357B-25CC-4bcf-96C1-78736985D412
          object pvaClsid = (object)(new Guid("C9A6357B-25CC-4bcf-96C1-78736985D412").ToString("B"));
         object pvarShow = (object)bShow;
         object pvarSize = null;
        
         if (bShow) /* hide Browser bar before showing to prevent erroneous behavior of IE*/
         {
            object pvarShowFalse = (object)false;
            explorer.ShowBrowserBar(ref pvaClsid, ref pvarShowFalse, ref pvarSize);
         }
         explorer.ShowBrowserBar(ref pvaClsid, ref pvarShow, ref pvarSize);
      }
      #endregion
   }
}

