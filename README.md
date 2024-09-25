

<div align="center">

  <h1 align="center">YT-DLP web app</h1>

  <p align="center">
    <a href="https://github.com/Azmekk/Magic-Number-Analyzer/issues">Report Bug</a>
    ·
    <a href="https://github.com/Azmekk/Magic-Number-Analyzer/issues">Request Feature</a>
  </p>
</div>

<div align="center">
  
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

[![dotNET][dotNET]][dotNET-url]
[![TypeScript][TypeScript]][TypeScript-url]
[![Svelte][Svelte]][Svelte-url]
[![TailwindCSS][TailwindCSS]][TailwindCSS-url]

</div>

## Overview

A web interface that leverages [YT-DLP](https://github.com/yt-dlp/yt-dlp)'s capabilities to make downloading and storing content user-friendly. It can be used as a standalone app that you run locally, or you can host it and use it on all of your devices. 
It features both light and dark themes and is designed with a mobile-friendly layout in mind, for smaller devices.

#### Layout preview
<div>
  <div style="flex: 1;">
  <img style="width: 80%" src="https://github.com/user-attachments/assets/78e39de7-085d-4db2-9697-ec85ed8af705" alt="Site_Desktop_Dark"></img>
  <img style="width: 19%" src="https://github.com/user-attachments/assets/8d4a277e-440a-49d1-a7c1-95936d82e72c" alt="Site_Mobile_Dark"></img>
  </div>
</div>

# Installation

## Running locally
If don't want to use docker, you can do the following: 
1. Download the application **for your platform** as a zip file from the [latest release](https://github.com/Azmekk/yt-dlp-web-app/releases/latest) 
2. Extract the zip using something like [7zip](https://www.7-zip.org/).
4. Run the file called `YT-DLP-Web-App-Backend.exe` and access the UI at http://localhost:41001

By default, the app will create necessary files in the same directory as the executable. To customize their locations, edit the .env file in the executable's directory. Here’s a breakdown:

## Docker Deployment

You can deploy the app using Docker by either pulling the image and running it with `docker run` or by using `docker-compose`. Here’s how to do both:

#### Using `docker run`

To start the container, use the following command:

```bash
docker run -d \
  --name yt-dlp-web-app \
  -p 41001:41001 \
  -v /hostpath/downloads:/app/yt-dlp-web/backend/Downloads \
  -v /hostpath/database:/app/yt-dlp-web/backend/Database \
  --restart unless-stopped \
  azmek/yt-dlp-web-app:latest
```

#### Using `docker-compose`
Alternatively, you can use Docker Compose to deploy the app. Create a docker-compose.yml file with the following content and run it using `docker-compose up -d`:

```yaml
services:
  yt-dlp-web-app:
    image: azmek/yt-dlp-web-app:latest
    ports:
      - 41001:41001
    volumes:
      - /hostpath/downloads:/app/yt-dlp-web/backend/Downloads
      - /hostpath/database:/app/yt-dlp-web/backend/Database
    restart: unless-stopped
```

# Migrating from old release

1. **BACKUP YOUR OLD DATABASE**
2. Run the new application at least once to generate an empty DB file which you can find in the `/Database` directory
3. Go to the [migrator release page](https://github.com/Azmekk/yt-dlp-web-app/releases/tag/v1.0-DatabaseMigrator) and grab the latest one for your paltform.
4. Copy the old and new database files somewhere convenient for you, run the application and follow the prompts.
5. The app will make 2 backup copies of both files just in case which you can later delete.
6. If you provided both files correctly, the app should migrate the data to the newer (Which should have been the auto generated one from step 2)
7. Move the new database file back to the `/Database` directory of your application.
8. Migration complete. You should now see all of your old videos.


## Bugs, issues, questions and requests

1. If you encounter any bug please check the [issues](https://github.com/Azmekk/Magic-Number-Analyzer/issues) page to see if it has been reported and/or fixed. 
2. If you determine that it hasn't feel free to open one.
3. If you would like to request a feature, feel free to open an issue, however I cannot promise it will be implemented.



[contributors-shield]: https://img.shields.io/github/contributors/Azmekk/yt-dlp-web-app.svg?style=for-the-badge
[contributors-url]: https://github.com/Azmekk/yt-dlp-web-app/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Azmekk/yt-dlp-web-app.svg?style=for-the-badge
[forks-url]: https://github.com/Azmekk/yt-dlp-web-app/network/members
[stars-shield]: https://img.shields.io/github/stars/Azmekk/yt-dlp-web-app.svg?style=for-the-badge
[stars-url]: https://github.com/Azmekk/yt-dlp-web-app/stargazers
[issues-shield]: https://img.shields.io/github/issues/Azmekk/yt-dlp-web-app.svg?style=for-the-badge
[issues-url]: https://github.com/Azmekk/yt-dlp-web-app/issues
[license-shield]: https://img.shields.io/github/license/Azmekk/yt-dlp-web-app.svg?style=for-the-badge
[license-url]: https://github.com/Azmekk/yt-dlp-web-app/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/Martin-Y
[dotNET]: https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=.net&logoColor=white
[dotNET-url]: https://dotnet.microsoft.com/en-us/
[TypeScript]: https://img.shields.io/badge/typescript-%23007ACC.svg?style=for-the-badge&logo=typescript&logoColor=white
[TypeScript-url]: https://www.typescriptlang.org/
[Svelte]: https://img.shields.io/badge/svelte-%23FF3E00.svg?style=for-the-badge&logo=svelte&logoColor=white
[Svelte-url]: https://svelte.dev/
[TailwindCSS]: https://img.shields.io/badge/tailwindcss-%2338B2AC.svg?style=for-the-badge&logo=tailwind-css&logoColor=white
[TailwindCSS-url]: https://tailwindcss.com/
