using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueWP.ATProto
{
  public class Settings
  {
    private const string _settingsFilename = "settings.dat";
    private string _selectedDID;
    private List<Credential> _credentials = new List<Credential>();

    public Settings()
    {
    }

    public string SelectedDID { get => _selectedDID; set => _selectedDID = value; }
    public List<Credential> Credentials { get => _credentials; set => _credentials = value; }

    public Credential CurrentCredential { get { return _credentials.FirstOrDefault(s => s.DID == _selectedDID); } }

    public async Task<bool> ReadCredentials()
    {
      try
      {
        var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        var provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider();

        var file = await localFolder.GetFileAsync(_settingsFilename);
        var buffProtected = await Windows.Storage.FileIO.ReadBufferAsync(file);

        var buffUnprotected = await provider.UnprotectAsync(buffProtected);
        var strClearText = Windows.Security.Cryptography.CryptographicBuffer.ConvertBinaryToString(Windows.Security.Cryptography.BinaryStringEncoding.Utf8, buffUnprotected);

        Newtonsoft.Json.JsonConvert.PopulateObject(strClearText,this);

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> WriteCredentials()
    {
      try
      {
        var str = Newtonsoft.Json.JsonConvert.SerializeObject(this);

        var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        var provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider("LOCAL=user");

        var buffMsg = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(str, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
        var buffProtected = await provider.ProtectAsync(buffMsg);

        var file = await localFolder.CreateFileAsync(_settingsFilename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
        await Windows.Storage.FileIO.WriteBufferAsync(file, buffProtected);

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> DeleteCredentials()
    {
      try
      {
        var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        _credentials.Remove(CurrentCredential);
        SelectedDID = _credentials.Count > 0 ? _credentials[0].DID : null;
        if (!await WriteCredentials())
        {
          return false;
        }
        /*
                var file = await localFolder.GetFileAsync(_settingsFilename);
                await file.DeleteAsync();
        */

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public class Credential
    {
      public string ServiceHost { get; set; }
      public string DID { get; set; }
      public string Handle { get; set; }
      public string AccessToken { get; set; }
      public string RefreshToken { get; set; }
    }
  }
}
