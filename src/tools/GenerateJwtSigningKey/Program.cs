using System.Security.Cryptography;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: dotnet run -- <output-pem-path>");
    return 1;
}

var outputPath = Path.GetFullPath(args[0]);
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

using var rsa = RSA.Create(2048);
var pkcs8 = rsa.ExportPkcs8PrivateKey();
var base64 = Convert.ToBase64String(pkcs8);
var lines = new List<string>();
for (var i = 0; i < base64.Length; i += 64)
    lines.Add(base64.Substring(i, Math.Min(64, base64.Length - i)));

var pem = "-----BEGIN PRIVATE KEY-----\n" + string.Join("\n", lines) + "\n-----END PRIVATE KEY-----\n";
File.WriteAllText(outputPath, pem);
Console.WriteLine("Wrote " + outputPath);
return 0;
