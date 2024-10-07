using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Wintrust;

namespace com.overwolf.dwnldr {
  public class SignitureHelper {

    // ------------------------------------------------------------------------
    public static Task<bool> IsSigned(string filePath) {
      return Task.Run(() => {
        return WinTrust.VerifyEmbeddedSignature(filePath);
      });
    }

    // ------------------------------------------------------------------------
    public static Task<string> GetCertHashString(string filePath) {
      return Task.Run(() => {
        try {
          X509Certificate signedFile = X509Certificate.CreateFromSignedFile(filePath);
          return signedFile.GetCertHashString();
        }
        catch {
          return null;
        }
      });
    }

    // ------------------------------------------------------------------------
    public static Task<string> GetSerialNumberString(string filePath) {
      return Task.Run(() => {
        try {
          X509Certificate signedFile = X509Certificate.CreateFromSignedFile(filePath);
          return signedFile.GetSerialNumberString();
        }
        catch {
          return null;
        }
      });
    }

    // ------------------------------------------------------------------------
    public static Task<bool> IsCertInstalled(string thumbprint, StoreName storeName) {
      return Task.Run(() => {
        try {
          X509Store store = new X509Store(storeName, StoreLocation.LocalMachine);
          store.Open(OpenFlags.ReadOnly);

          var CERTIFICATES = store.Certificates.Find(
            X509FindType.FindByThumbprint,
            thumbprint,
            false
          );

          if (CERTIFICATES == null || CERTIFICATES.Count == 0) {
            return false;
          }

          foreach (var cert in CERTIFICATES) {
            X509Chain chain = new X509Chain();
            bool valid = chain.Build(cert);
            if (!valid) {
              return false;
            }

          }

          return true;
        } catch (Exception ex) {
          return false;
        }
      });

    }
  }
}
