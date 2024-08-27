package utils

import (
	"encoding/json"
	"fmt"
	"net/http"
	"path/filepath"
)

func WriteJsonResponse(responseStruct any, w http.ResponseWriter) {
	response, err := json.Marshal(responseStruct)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		fmt.Println("Failed to serialize struct when writing json response.")
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.Write(response)
}

func WriteJsonError(w http.ResponseWriter, stringErr string, status int) {
	errResponseStruct := ErrorResponse{
		Error: stringErr,
	}

	response, err := json.Marshal(errResponseStruct)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		fmt.Println("Failed to serialize struct when writing json response.")
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	w.Write(response)
}

func WriteEmptySuccess(w http.ResponseWriter) {
	w.WriteHeader(http.StatusOK)
	w.Write([]byte("Success!"))
}

func FileDownloadResponse(w http.ResponseWriter, r *http.Request, path string) {
	w.Header().Set("Content-Disposition", fmt.Sprintf("attachment; filename=%s;", filepath.Base(path)))
	http.ServeFile(w, r, path)
}

func FileResponse(w http.ResponseWriter, r *http.Request, path string) {
	w.Header().Set("Content-Disposition", fmt.Sprintf("inline; filename=%s;", filepath.Base(path)))
	http.ServeFile(w, r, path)
}
