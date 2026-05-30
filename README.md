# KFD EKC Editor/Keyloader

This is a fork of the original Omahacomsys KFDtool UI tool (https://github.com/omahacommsys/KFDtool). The primary purpose of this fork is for implementing remote management via DVM FNE's metadata network interface of EKC containers used on FNE installations. This 
is not a replacement for the original KFDtool and should not be treated as such, while it has the same feature set, and supports keyloading via the KFDshield and KFDshield-clone devices, it does not support the original KFDtool device (this was done to remove HidLibrary as 
a dependancy). Additionally for the DVM FNE integration, adds support to maintain, create, edit, delete UKEK and LLA keys.

## Building

This project utilizes a standard Visual Studio solution for its build system.

### Build Instructions

1. Clone the repository. `git clone --recurse-submodules https://github.com/DVMProject/kfdekc.git`
2. Switch into the "kfdekc" folder.
3. Open the "KFDEKC.sln" with Visual Studio.
4. Compile.

## License

This project is licensed under the MIT License – see the [LICENSE](LICENSE) file for details.

This software is intended for amateur and/or educational use. Any other use is at the user's discretion and risk. Commercial use is strongly discouraged.
