terraform {
  required_providers {
    google-beta = {
      source  = "hashicorp/google-beta"
      version = "~> 4.0"
    }
  }
}

provider "google-beta" {
  billing_project       = local.unrelated_gcp_project_to_verify_billing
  user_project_override = true
}

###############
# Setup Project
###############
resource "google_project" "default" {
  provider = google-beta

  name            = local.project_display_name
  project_id      = local.project_id
  billing_account = local.billing_account

  labels = {
    "firebase" = "enabled"
  }
}

resource "google_project_service" "default" {
  provider = google-beta
  project  = google_project.default.project_id

  for_each = toset([
    "cloudbilling.googleapis.com",
    "cloudresourcemanager.googleapis.com",
    "firebase.googleapis.com",
    "firebaserules.googleapis.com",
    "firestore.googleapis.com",
    "identitytoolkit.googleapis.com",
    "serviceusage.googleapis.com",
  ])
  service = each.key

  # Don't disable the service if the resource block is removed by accident.
  disable_on_destroy = false
}

resource "google_firebase_project" "default" {
  provider = google-beta

  project = google_project.default.project_id

  depends_on = [
    google_project.default,
    google_project_service.default
  ]
}

#################
# Setup Firestore
#################
resource "google_firestore_database" "firestore" {
  provider = google-beta
  project  = google_project.default.project_id

  name = "(default)"
  location_id                 = local.gcp_location
  type                        = "FIRESTORE_NATIVE"
  concurrency_mode            = "OPTIMISTIC"
  app_engine_integration_mode = "DISABLED"

  depends_on = [
    google_project_service.default,
  ]
}

resource "google_firebaserules_ruleset" "firestore" {
  provider = google-beta
  project  = google_project.default.project_id

  source {
    files {
      name    = "firestore.rules"
      content = file("firestore.rules")
    }
  }

  depends_on = [
    google_firestore_database.firestore,
  ]
}

resource "google_firebaserules_release" "firestore" {
  provider = google-beta
  project  = google_project.default.project_id

  name         = "cloud.firestore" # must be cloud.firestore
  ruleset_name = google_firebaserules_ruleset.firestore.name

  depends_on = [
    google_firestore_database.firestore,
  ]
}

############
# Setup Auth
############
resource "google_identity_platform_config" "auth" {
  provider = google-beta
  project  = google_project.default.project_id

  depends_on = [
    google_project_service.default,
  ]
}

resource "google_identity_platform_project_default_config" "auth" {
  provider = google-beta
  project  = google_project.default.project_id
  sign_in {
    allow_duplicate_emails = false

    anonymous {
      enabled = true
    }
  }

  depends_on = [
    google_identity_platform_config.auth,
  ]
}

#############
# Create Apps
#############
resource "google_firebase_android_app" "app" {
  provider = google-beta
  project  = google_project.default.project_id

  display_name = local.android_display_name
  package_name = local.android_package_name

  depends_on = [
    google_firebase_project.default,
  ]
}

resource "google_firebase_apple_app" "app" {
  provider = google-beta
  project  = google_project.default.project_id

  display_name = local.ios_display_name
  bundle_id    = local.ios_package_name

  depends_on = [
    google_firebase_project.default,
  ]
}

resource "google_firebase_web_app" "app" {
  provider = google-beta
  project  = google_project.default.project_id

  display_name = local.web_display_name
  # The other App types (Android and Apple) use "DELETE" by default.
  # Web apps don't use "DELETE" by default due to backward-compatibility.
  deletion_policy = "DELETE"

  depends_on = [
    google_firebase_project.default,
  ]
}