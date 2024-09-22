package main

import (
	"fmt"
	"log"
	"net/http"
	"os"
	"yt-dlp/web/database"
	"yt-dlp/web/routes"
	"yt-dlp/web/utils"
)

func main() {

	utils.AdjustDefaultDirsBasedOnOs()
	err := utils.ReadAndSetEnvVars()
	if err != nil {
		fmt.Println("Failed to read and/or set .env variables:", err)
		return
	}

	utils.InitiateRand()
	err = utils.CheckDependencies()
	if err != nil {
		fmt.Println("Failed to find dependencies:", err)
		return
	}

	err = os.MkdirAll(utils.DefaultDownloadDir, os.ModePerm)
	if err != nil {
		fmt.Println("Error creating default download directory:", err)
		return
	}

	err = os.MkdirAll(utils.DefaultStaticDir, os.ModePerm)
	if err != nil {
		fmt.Println("Error creating default static directory:", err)
		return
	}

	err = database.MigrateAndInitiateDbConn()
	if err != nil {
		fmt.Println("Error when initiating sqllite DB:", err)
		return
	}

	fmt.Println("Starting server")

	routes.RegisterRoutes()

	fmt.Println("Running on http://localhost:" + utils.Port)
	log.Fatal(http.ListenAndServe(":"+utils.Port, nil))
}
