@Echo Off

SetLocal EnableExtensions
Cd /d "%~dp0"

For /f "delims=" %%a in ('Date /t') Do (Set mdate=%%a)
For /f "delims=" %%a in ('Time /t') Do (Set mtime=%%a)
Set mdate=%mdate: =_%
Set mdate=%mdate::=_%
Set mdate=%mdate:/=_%
Set mtime=%mtime: =_%
Set mtime=%mtime::=_%
Set mtime=%mtime:/=_%

CoverMe>log_%mdate%_%mtime%.txt