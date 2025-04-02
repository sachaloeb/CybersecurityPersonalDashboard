using System;
using System.Runtime.InteropServices;

public static class SecurityOverviewServiceFactory
{
    public static ISecurityOverviewService Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new SecurityOverviewServiceWindows();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new SecurityOverviewServiceLinux();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new SecurityOverviewServiceMac();
        }
        
        // If you need to handle other OSes, add them here
        throw new PlatformNotSupportedException("Unknown or Unsupported OS");
    }
}