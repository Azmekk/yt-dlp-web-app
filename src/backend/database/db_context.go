package database

import (
	"fmt"
	"os"
	"path/filepath"
	"yt-dlp/web/utils"

	"gorm.io/driver/sqlite"
	"gorm.io/gorm"
)

var DbConn *gorm.DB

func MigrateAndInitiateDbConn() error {
	fmt.Println("Connecting to database...")
	err := os.MkdirAll(utils.DefaultDatabaseDir, os.ModePerm)
	if err != nil {
		return err
	}

	db, err := gorm.Open(sqlite.Open(filepath.Join(utils.DefaultDatabaseDir, "app_database.db")), &gorm.Config{})
	if err != nil {
		return err
	}

	DbConn = db

	err = DbConn.AutoMigrate(&Video{})
	if err != nil {
		return err
	}

	fmt.Println("Done!")
	return nil
}
