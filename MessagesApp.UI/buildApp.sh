#!/bin/bash

# Paths
app_name="MessagesApp.UI"                
output_app_name="MsgApp"                 
app_folder="./Builds/${output_app_name}.app"           
macos_folder="${app_folder}/Contents/MacOS"
resources_folder="${app_folder}/Contents/Resources"
plist_file="${app_folder}/Contents/Info.plist"

# Clean up the previous app folder if it exists
rm -rf "$app_folder"

# Clean, build, and publish the application
dotnet clean
dotnet build -c Release
dotnet publish -c Release -r osx-arm64 --self-contained true

# Determine the publish folder based on the output
publish_folder="./bin/Release/net8.0/osx-arm64/publish"

# Create the folder structure
mkdir -p "$macos_folder"
mkdir -p "$resources_folder"
mkdir -p "$(dirname "$plist_file")"

# Copy content from the publish folder to the .app structure
cp -R "$publish_folder/"* "$macos_folder/"
sync && sleep 1  # Ensuring files are fully copied before moving on

# Move resources like JSON files
mv "$macos_folder/"*.json "$resources_folder/"
sync && sleep 1  # Ensure file move is completed

# Rename the executable
mv "$macos_folder/$app_name" "$macos_folder/$output_app_name"

# Ensure the executable has the right permissions
chmod +x "$macos_folder/$output_app_name"

# Create the Info.plist file
cat > "$plist_file" <<EOL
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>${output_app_name}</string>  
    <key>CFBundleIdentifier</key>
    <string>com.yourcompany.${output_app_name}</string>
    <key>CFBundleName</key>
    <string>${output_app_name}</string>
    <key>CFBundleVersion</key>
    <string>1.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.13</string>
</dict>
</plist>
EOL

# Sign the .app bundle
echo "Signing the app bundle..."
codesign --force --deep --sign - "$app_folder"

# Print the structure to verify the contents
echo "Created the app bundle: $app_folder"
find "$app_folder" -print