locals {
  # Required values to set
  unrelated_gcp_project_to_verify_billing = "ludum-dare-383812"    # From https://console.cloud.google.com/billing/projects
  billing_account                         = "000000-AAAAAA-111111" # From https://console.cloud.google.com/billing
  project_display_name                    = "Ludum Dare Template 9"
  project_id                              = "ludum-dare-template-9"

  android_display_name = "Ludum Dare Template - Android"
  android_package_name = "com.baconetastudios.template" # Must match the value set in Unity (Project Settings | Player | Other Settings | Android | Package Name)
  ios_display_name     = "Ludum Dare Template - iOS"
  ios_package_name     = "com.baconetastudios.template" # Must match the value set in Unity (Project Settings | Player | Other Settings | iOS | Bundle Identifier)
  web_display_name     = "Ludum Dare Template - Web"

  # Optional values to set
  gcp_location = "northamerica-northeast1" # See available locations: https://firebase.google.com/docs/projects/locations#default-cloud-location
}