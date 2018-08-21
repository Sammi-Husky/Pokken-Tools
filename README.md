# Pokken-Tools
Collection of Pokken tools for Decrypting, Unpacking, Repacking, and Re-Encrypting Pokken DRP archives.

## Requirements
- .NET Framework 4.0
- Visual Studio 2015

## Contents
- **DRPDecrypter**
  - CLI Application for decrypting and re-encrypting DRP(.drp) archive files.
  
- **DRPExtractor**
  - Application for extracting DRP archive
  
- **DRPRepacker**
  - CLI Application for patching and updating files in DRP archives.
   - ```
     Usage: DRPRepacker <decrypted drp file> <folder containing files>
     ```
- **DRPExplorer**
  - WIP and currently unusable Explorer application for browsing, editing, and creating DRP archives with a GUI.
  
## Building
  - Checkout the repo: `git checkout https://github.com/Sammi-Husky/Sm4sh-Tools.git`
  - Use the Solution file to build the projects.
  
## Credits 
  - All code is Copyright (c) 2018 - Sammi Husky, unless otherwise stated in project READMEs
  - Some projects make use of Open Source components; See COPYING in the respective project's project directory for more information.
  - Special thanks:
    - **DSX8**: for his days worth of testing and sending files back and forth with me.
    - **RandomTalkingBush**: for his extraction BMS scripts and other work.
    - **Dantarion**: for helping me with the initial Reverse Engineering of the Pokken Encryption Algorithm.
  
## License 
  - For specific License information please refer to the LICENSE file in each project's project directory. If one does not exist, the code is licensed to the public domain.
