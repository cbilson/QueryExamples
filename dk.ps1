#$repo = 'http://svn.prod.ds.russell.com/svn/gad/trunk/Common/ThirdParty/'
$repo = 'file://C:/Users/CBilson/Documents/Projects/Common/ThirdParty/'

$packages = @{

    'Russell-Common' = @{
        Folder = 'Russell';
        Components = @('Ris.Common')
     };

     'Russell-Testing' = @{
        Folder = 'Russell';
        Components = @('Ris.Common.Tests')
     };

    Castle = @{
        Folder = "Castle";
        Components = @('Castle.Core', 'Castle.DynamicProxy2', 
                       'Castle.MicroKernel', 'Castle.Windsor')
    };

    Monorail = @{
        Folder = "Castle";
        Components = @('Castle.Components.Validator', 'Castle.MonoRail.Framework', 
                       'Castle.MonoRail.TestSupport', 'Castle.MonoRail.WindsorExtension',
                       'Newtonsoft.Json', 'Castle.Components.Binder',
			'Castle.Components.Common.EmailSender',
			'Castle.Components.DictionaryAdapter',
			'Castle.Components.Pagination',
                        'Antlr3.Runtime')
    };

    Brail = @{
        Folder = "Castle";
        Components = @('Castle.Monorail.Views.Brail', 'Boo.Lang', 
                       'Boo.Lang.Compiler', 'Boo.Lang.Parser', 'anrControls.Markdown',
					   'Boo.Lang.Extensions')
    };

    Spark = @{
        Folder = "Spark";
        Components = @('Castle.MonoRail.Views.Spark', 'Spark')
    };

    'Castle-Logging' = @{
        Folder = "Castle";
        Components = @('Castle.Facilities.Logging', 'Castle.Services.Logging.Log4netIntegration')
    };

    'Castle-Scheduler' = @{
        Folder = 'Castle';
        Components = @('Castle.Components.Scheduler', 
                       'Castle.Components.Scheduler.WindsorExtension')
    };

    'Castle-WCF' = @{
        Folder = 'Castle';
        Components = @('Castle.Facilities.WcfIntegration')
    };

    NHibernate = @{
        Folder = "NHibernate";
        Components = @('Iesi.Collections', 'NHibernate.ByteCode.Castle', 
                       'NHibernate')
    };

    'NHibernate-Caching' = @{
        Folder = "nhcontrib";
        Components = @('NHibernate.Caches.SysCache2')
    };

    'NHibernate-Linq' = @{
        Folder = "nhcontrib";
        Components = @('NHibernate.Linq')
    };

    'Fluent-NHibernate' = @{
        Folder = "Fluent-NHibernate";
        Components = @('FluentNHibernate', 'FluentNHibernate.Testing')
    };

    'Service-Locator' = @{
        Folder = "CommonServiceLocator";
        Components = @('Microsoft.Practices.ServiceLocation', 'CommonServiceLocator.WindsorAdapter')
    };

    Topshelf = @{
        Folder = "Topshelf";
        Components = @('Topshelf')
    };

    Magnum = @{
        Folder = "Magnum";
        Components = @('Magnum', 'Magnum.Infrastructure')
    };

    'Mass-Transit' = @{
        Folder = "MassTransit";
        Components = @('MassTransit', 'MassTransit.Infrastructure', 
                       'MassTransit.RuntimeServices', 'MassTransit.Transports.Msmq',
                       'MassTransit.WindsorIntegration', 'SubscriptionServiceHost',
					   'MassTransit.StructureMapIntegration', 'StructureMap')
    };

    'Fluent-Migrator' = @{
        Folder = "FluentMigrator";
        Components = @('FluentMigrator', 'FluentMigrator.Testing', 
                       'FluentMigrator.Console', 'FluentMigrator.Runner')
    };

    Gallio = @{
        Folder = 'Gallio';
        Components = @('Gallio.Echo', 'Gallio.Host', 'Gallio.Reports', 'Gallio',
                       'Gallio35', 'MbUnit.Compatibility', 'MbUnit',
                       'MbUnit35')
    };

    'Machine-Specifications' = @{
        Folder = 'machine.specifications';
        Components = @('Machine.Specifications.ConsoleRunner',
                       'Machine.Specifications',
                       'Machine.Specifications.GallioAdapter',
                       'Machine.Specifications.Reporting',
                       'CommandLine')
    };

    'Rhino-Mocks' = @{
        Folder = 'Rhino-Mocks';
        Components = @('Rhino.Mocks');
    };

    log4net = @{
        Folder = 'log4net';
        Components = @('log4net')
    };

    sqlite = @{
        Folder = 'sqlite';
        Components = @('sqlite3', 'System.Data.SQLite')
    };

    'Aspose-Cells' = @{
        Folder = 'Aspose';
        Components = @('Aspose.Cells', 'Aspose.Total')
    };

    'Aspose-Chart' = @{
        Folder = 'Aspose';
        Components = @('Aspose.Chart', 'Aspose.Total')
    };

    'Aspose-Pdf' = @{
        Folder = 'Aspose';
        Components = @('Aspose.Pdf', 'Aspose.Total')
    };
}

function Get-ComponentName($nameLikeThing) {
    if ($component -is [io.FileInfo]) {
        $name = $component.Name
    } else {
        $name = $component
    }  

    return [io.Path]::GetFileNameWithoutExtension($name)
}

function Get-PackageForComponent ($component) {
    $name = Get-ComponentName $component

    ForEach ($package in $packages.GetEnumerator()) {
        if ($package.Value.Components -contains $name) {
            return $package.Name
        }
    }

    return "*** No Package for $name ***"
}

function Get-InstalledPackages($libDir) {
    $names = @()

    Get-ChildItem $libDir `
       | Where-Object { $_.Name.EndsWith('.dll') -or $_.Name.EndsWith('.exe') } `
       | ForEach-Object { Get-PackageForComponent $_ } `
       | ForEach-Object { if ($names -notcontains $_) { $names += $_ } }
       
    $names
}

function Get-AllPackageNames() {
    ForEach ($def in $packages.GetEnumerator()) { $def.Name }
}

function Try-DownloadFile($url, $cred) {
    $tmp = [io.Path]::Combine([io.Path]::GetTempPath(), 
                              [io.Path]::GetRandomFilename())
    $uri = New-Object Uri($url)

    if ($uri.Scheme -eq 'file') {
        if (!(Test-Path $uri.LocalPath)) {
            return $null
        }
    }

    $request = [net.WebRequest]::Create($url)

    if ($cred -ne $null) { $request.Credentials = $cred }    
        
    trap {
        if ($error[0].Exception.Message.Contains("404")) { return $null }
        throw
    }

    $response = $request.GetResponse()

    $responseStream = $response.GetResponseStream()
    $file = New-Object io.FileStream($tmp, [io.FileMode]::Create)
    
    trap { $file.Close() }
    $buffer = New-Object byte[] 8192
    
    $read = 0
    do {
        $read = $responseStream.Read($buffer, 0, 8192)
        $file.Write($buffer, 0, $read)
    } while ($read -ne 0)

    $file.Close()
    return $tmp
}

function Get-Sha1($fileName) {
    $algo = [System.Security.Cryptography.HashAlgorithm]::Create('sha1')
    $mode = [io.FileMode]::Open
    $stm = New-Object System.IO.FileStream $filename, $mode

    try {
       if ($stm.Length -gt 0) {
           $hash = $algo.ComputeHash($stm)
           $hashString = ""
           ForEach ($byte in $hash) { $hashString += $byte.ToString("x2") }
           $hashString
       }
    }

    finally { $stm.Close() }
}

function Replace-ComponentIfNotSame($target, $latest) {

    if (Test-Path $target) {

       $latestHash = Get-Sha1 $latest
       $targetHash = Get-Sha1 $target
    
      if ($latestHash -eq $targetHash) {
          "Up To Date: $target"
          return
      } else {
          "UPDATE: $target"
      }
    } else {
      "NEW: $target"
    }

    Copy-Item -Force $latest $target
}

function Update-Package($packageName, $root, $cred) {
    if ($packages.ContainsKey($packageName) -ne $true) {
        throw "No package $packageName defined. Try Get-AllPackageNames to see a list."
    }

    if ($root -eq $null) {
        if (Test-Path 'lib') { $root = Resolve-Path 'lib' }
        elseif (Test-Path 'ThirdParty') { $root = 'ThirdParty' }
    }

    $def = $packages[$packageName]
    
    ForEach ($component in $def.Components) {
        $baseurl = "{0}{1}/{2}" -f $repo, $def.Folder, $component
        ForEach ($extension in @('dll', 'pdb', 'exe', 'dll.config', 'exe.config', 'plugin', 'lic')) {
            $fileName = $component + '.' + $extension
            $target = [io.Path]::Combine($root, $fileName)
            $url = $baseurl + '.' + $extension
            $tmp = Try-DownloadFile $url $cred
            if ($tmp -ne $null) {
                if (Test-Path $tmp) {
                    try { Replace-ComponentIfNotSame $target $tmp }
                    finally { Remove-Item $tmp }
                } else { "didn't find $url" }
            }
        }
    }
}

function Add-Package($packageName, $root, $cred) {
    Update-Package($packageName, $root, $cred)
}

function Update-AllPackages($root, $cred) {
    Get-InstalledPackages $root `
    | ForEach-Object { Update-Package $_ $root $cred }
}

#Get-InstalledPackages 'lib'
#Update-AllPackages 'lib' $(Get-Credential)