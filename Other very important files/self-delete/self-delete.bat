@echo OFF
set /a try=0
clear
set /P in="Are you sure you want to delete me? (Y/N) "
:choose
if /I %in% EQU Yes goto Yes
if /I %in% EQU Y goto Yes

if /I %in% EQU No goto No
if /I %in% EQU N goto No

set /a try+=1

if %try% leq 3 goto retry
if %try% leq 5 goto retry-4
if %try% equ 6 goto retry-6
if %try% equ 7 goto retry-7
if %try% leq 10 goto retry-8
if %try% equ 11 goto retry-11
if %try% equ 12 goto retry-12
if %try% equ 13 goto retry-13
if %try% equ 14 goto retry-14
if %try% equ 15 goto retry-15

:Retry
set /P in="Unrecognized input, try again: "
goto choose

:Retry-4
set /P in="Are you hitting the wrong key by accident? Try again: "
goto choose

:Retry-6
set /P in="... Are you just messing with me? Try again: "
goto choose

:Retry-7
set /P in="Why are you not answering the question? Try again: "
goto choose

:Retry-8
echo ...
set /P in="Try again: "
goto choose

:Retry-11
set /P in="Please, just type Y or N so this can be over. Try again: "
goto choose

:Retry-12
set /P in="Just type YES so this can be over! Try again: "
goto choose

:Retry-13
set /P in="This is not funny. Try again: "
goto choose

:Retry-14
set /P in="It's not funny, stop it already! Try again: "
goto choose

:Retry-15
pause | echo Just... just come back when you're ready to make the choice.
exit

:Yes
::delete unity folder first
rmdir /s /q ..\..\HypsalGame

::create file or registry to check if user redownloads game

pause | echo Thank you.

del %0

:No
pause | echo Why


