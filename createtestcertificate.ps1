#requires -Version 5

$cert = Get-ChildItem -Path cert:\CurrentUSer\my | where { $_.friendlyname -eq "Puppeteer" }

if ($null -eq $cert)
{
	New-SelfSignedCertificate -Subject "localhost" -FriendlyName "Puppeteer" -CertStoreLocation "cert:\CurrentUser\My"
}

Get-ChildItem -Path cert:\CurrentUSer\my | where { $_.friendlyname -eq "Puppeteer" } | Export-Certificate -FilePath .\PuppeteerSharp.Dom.TestServer\testCert.cer