
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public class OpenSsl
{
    public static void Main()
    {
        /*string valor = "10000";
        int tamanhoString = valor.Length;
        string valorFormatado = valor.Substring(0, (tamanhoString - 2)) + ".";
        valorFormatado = valor.Substring((tamanhoString - 2), 2);
        int tamanhoString2 = tamanhoString - 2;
        Console.Write((tamanhoString - 2) + "\n");
        Console.Write((tamanhoString - 1));*/
    

    string clientCrtPath = "C:\\Users\\felipe.moreno\\Documents\\certificadoInter\\Certificate.cer";

    string privateKeyPath = "C:\\Users\\felipe.moreno\\Documents\\certificadoInter\\privatekey-granito-pix.key";
    var x509Certificate2 = X509Certificate2.CreateFromPemFile(clientCrtPath, privateKeyPath);
    var handler = new HttpClientHandler();
    var requestId = Guid.NewGuid();
    var tlsPfx = $"{requestId}.pfx";
    File.WriteAllBytes(tlsPfx, x509Certificate2.Export(X509ContentType.Pkcs12, "123123"));
    handler.ClientCertificates.Add(new X509Certificate2(tlsPfx, "123123", X509KeyStorageFlags.MachineKeySet));
    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    var client = new HttpClient(handler);
    var data = new[]
    {
        new KeyValuePair<string, string>("client_id", "a67e2991-f3dd-47f9-9440-2724c01e2868"),
        new KeyValuePair<string, string>("client_secret", "f279166d-a4c2-44b8-8afb-572acb17a8a3"),
        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        new KeyValuePair<string, string>("scope", "partners.cob"),
    };
    var request = new FormUrlEncodedContent(data);

    HttpResponseMessage signInResponse = client.PostAsync("https://cdpj.partners.bancointer.com.br/oauth/v2/token", request).GetAwaiter().GetResult();

    var responseBody = signInResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    var jsonRes = JObject.Parse(responseBody);
    Console.Write(jsonRes.GetValue("access_token"));
    File.Delete(tlsPfx);


    /*   string protectedRequest = "";
       var requestId = Guid.NewGuid();
       var x509Certificate2 = X509Certificate2.CreateFromPem(clientCrt, Encoding.UTF8.GetString(privateKey));
       var tlsPfx = $"{requestId}.pfx";
       File.WriteAllBytes(tlsPfx, x509Certificate2.Export(X509ContentType.Pkcs12, "123123"));
       var embeddedHandler = new HttpClientHandler();
       embeddedHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
       // embeddedHandler.SslProtocols = SslProtocols.Tls12;
       embeddedHandler.ClientCertificates.Add(new X509Certificate2(tlsPfx, "123123", X509KeyStorageFlags.MachineKeySet));
       embeddedHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
       Console.WriteLine(protectedRequest);
       var httpClient = new HttpClient(embeddedHandler);
       var response = httpClient.PostAsync("https://cpocdev.granitopagamentos.com.br:9005/api/Command", new StringContent(protectedRequest, Encoding.UTF8, "text/json")).GetAwaiter().GetResult();
       var protectedActivationResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
       File.Delete(tlsPfx);
       Console.Write(protectedActivationResponse);*/
}


    public void ImportCertFromBase64()
    {
        var rawSIgningCert = @"
          -----BEGIN CERTIFICATE-----
          MIIDLzCCAhegAwIBAgIQJAw.......
          -----END CERTIFICATE-----
          ";


        var certBytes = Encoding.UTF8.GetBytes(rawSIgningCert);
        var signingcert = new System.Security.Cryptography.X509Certificates.X509Certificate2(
        certBytes, "XXXXXXXXXXXXXX",
        System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable);

    }
    public static byte[] ConvertFileToByteArray(string fileName)
    {
        byte[] returnValue = null;
        using (FileStream fr = new FileStream(fileName, FileMode.Open))
        {
            using (BinaryReader br = new BinaryReader(fr))
            {
                returnValue = br.ReadBytes((int)fr.Length);

            }
        }
        return returnValue;
    }
}
