@echo off

set "SOURCE_FILE=ServerSubscriptionManager.db"
set "BACKUP_FOLDER=Backups"
set "BACKUP_DATE=%date:~10,4%-%date:~4,2%-%date:~7,2%" :: Formatted as YYYY-MM-DD

copy "%SOURCE_FILE%" "%BACKUP_FOLDER%\%BACKUP_DATE%_%SOURCE_FILE%"
