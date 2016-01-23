if not exist "C:\Program Files\Hubble Desktop" mkdir "C:\Program Files\Hubble Desktop"
if not exist "C:\Program Files\Hubble Desktop\wallpapers" mkdir "C:\Program Files\Hubble Desktop\wallpapers"
copy "%~dp0HubbleDesktop\bin\Debug\HubbleDesktop.exe" "C:\Program Files\Hubble Desktop\HubbleDesktop.exe"

echo Didn't work? Try running "auto_admin_installer" instead
pause