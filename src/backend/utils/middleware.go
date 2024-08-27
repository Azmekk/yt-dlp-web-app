package utils

import (
	"net/http"
)

func HandleFuncWithMiddleware(pattern string, handler func(http.ResponseWriter, *http.Request)) {
	passedFuncHandler := http.HandlerFunc(handler)
	http.Handle(pattern, CorsMiddleware(passedFuncHandler))
}

func CorsMiddleware(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		returnOrigin := "http://localhost:5173"
		origin := r.Header.Get("Origin")

		returnOrigin = origin

		w.Header().Set("Access-Control-Allow-Origin", returnOrigin)
		w.Header().Set("Access-Control-Allow-Credentials", "true")
		w.Header().Set("Access-Control-Allow-Headers", "Content-Type")
		w.Header().Set("Access-Control-Allow-Methods", "GET, PUT, POST, DELETE, HEAD, OPTIONS")

		if r.Method == http.MethodOptions {
			w.WriteHeader(http.StatusOK)
			return
		}

		next.ServeHTTP(w, r)
	})
}
