// Prevents additional console window on Windows in release, DO NOT REMOVE!!
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

use std::os::windows::process::CommandExt;
use std::{
    process::{Child, Command},
    sync::{Arc, Mutex},
};

fn spawn_child_process() -> Child {
    #[cfg(target_os = "windows")]
    let child = Command::new(".\\_root_\\app\\YT-DLP-Web-App-Backend.exe")
        .creation_flags(0x08000000)
        .spawn()
        .expect("Failed to start child process on Windows");

    #[cfg(target_os = "linux")]
    let child = Command::new("./_root_/app/YT-DLP-Web-App-Backend")
        .creation_flags(0x08000000)
        .spawn()
        .expect("Failed to start child process on Linux");

    child
}

fn main() {
    let child = Arc::new(Mutex::new(spawn_child_process()));
    tauri::Builder::default()
        .on_window_event(move |event| {
            if let tauri::WindowEvent::CloseRequested { .. } = event.event() {
                let _ = child.lock().unwrap().kill();
            };
        })
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
