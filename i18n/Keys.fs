module Antidote.FormStudio.i18n.Keys

type Intl =
    | Dashboard
    | SuicidalScreenings
    | ThisLanguage
    | English
    | Spanish
    | French
    | Male
    | Female
    | SignIn
    | SignInGreeting
    | CreateAccount
    | Schedule
    | SetSchedule
    | SaveAppointment
    | Members
    | Providers
    | Campaign
    | Calendar
    | RefreshMembers
    | History
    | Upcoming
    | Search
    | SignOut
    | Login
    | About
    | FrontCard
    | BackCard
    | Password
    | ConfirmPassword
    | ChangePassword
    | ChoosePassword
    | ForgotPassword
    | NewPassword
    | ResetPassword
    | FirstName
    | LastName
    | DateOfBirth
    | Username
    | Email
    | PhoneNumber
    | MobilePhoneNumber
    | HomePhoneNumber
    | PrimaryContact
    | Address
    | State
    | VisitType
    | VisitTypeSelect
    | Facility
    | FacilitySelect
    | VisitReason
    | Next
    | Previous
    | Submit
    | Register
    | Gender
    | Patient
    | Form
    | SOAPNotes
    | Date
    | Time
    | Location
    | Flags
    | Severity
    | Unspecified
    | Mild
    | Moderate
    | Suicidal
    | Points
    | InHouseClinicianEnrollment

    //Forms
    | SelectPlaceholder

    | ProviderCharting
    | Results
    | ClinicalNotes
    | Clinician
    | RecommendedCodes
    | DownloadPdf
    | NoActiveRecommendedCodes
    | SuicidalWarning
    | FunctionalPatient
    | ScreeningZeroToFour
    | ScreeningFiveToNine
    | ScreeningTenToFourteen
    | ScreeningFifteenToNineteen
    | ScreeningTwentyOrGreater

    | AppointmentDetails
    | AppointmentDetailsSubtitle

    | InOffice
    | TeleHealth

    | ThankYou
    | ThankYouMessage
    | ThankYouPleaseReturnDevice
    | ThankYouGetUpdatesMessage
    | ThankYouDownloadApp

    | SignUpLetsGo
    | SignUpPreferredLanguage
    | SignUpSubtitle
    | SignUpSpokenLanguageStepSubtitle
    | SignUpLoginInfo
    | SignUpLoginInfoSubtitle
    | SignUpNameTitle
    | SignUpInsuranceTitle
    | SignUpAddressTitle
    | SignUpDobTitle
    | SignUpGenderTitle

    | LandingContinueWithAntidote

    | ForgotPasswordSendEmail

    | SocialSecurityNumber
    | InsuranceCarrierName
    | InsuranceGroupNumber
    | InsurancePlanName
    | InsuranceNumber
    | InsuranceMemberId
    | TelephoneNumber
    | StreetAddress1
    | StreetAddress2
    | City
    | ZipCode
    | ReferringReason
    | Status

    | Avatar

    | StatusOnline
    | StatusOffline
    | StatusDoNotDisturb
    | StatusInCall

    | HandoutModeEnter

    | NavbarIntakeHistory
    | NavbarPatientIntakeHistory
    | NavbarProviderIntakeHistory
    | NavbarReferralIntakeHistory
    | NavbarVisitHistory
    | NavbarDeleteAccount

    //Site Map
    | PageHealixHome
    | PageUserProfile
    | PagePatientChart

    | ProfileGreeting

    | AboutFindUsAppleStore
    | AboutReviewActivity
    | AboutScanQrToGetStarted

    | January
    | February
    | March
    | April
    | May
    | June
    | July
    | August
    | September
    | October
    | November
    | December

    | Sunday
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday

    | Today
    | Past
    | Yesterday
    | Tomorrow

    //Seattle 7
    | DailyAngina
    | WeeklyAngina
    | MonthlyAngina
    | NoAngina

    //CAT
    | CatLowRisk
    | CatModerateRisk
    | CatHighRisk
    | CatVeryHighRisk


    member this.Key =
        let key =
            match this with
            | Dashboard -> "dashboard"
            | SuicidalScreenings -> "suicidal_screenings"
            | ThisLanguage -> "this_language"
            | English -> "english"
            | Spanish -> "spanish"
            | French -> "french"
            | Male -> "male"
            | Female -> "female"
            | SignIn -> "sign_in"
            | SignInGreeting -> "sign_in_greeting"
            | CreateAccount -> "create_account"
            | Schedule -> "schedule"
            | SetSchedule -> "set_schedule"
            | SaveAppointment -> "save_appointment"
            | Members -> "members"
            | Providers -> "providers"
            | Campaign -> "campaign"
            | Calendar -> "calendar"
            | RefreshMembers -> "refresh_members"
            | History -> "history"
            | Upcoming -> "upcoming"
            | Search -> "search"
            | SignOut -> "sign_out"
            | About -> "about"
            | Login -> "login"
            | Password -> "password"
            | ConfirmPassword -> "confirm_password"
            | ChangePassword -> "change_password"
            | ChoosePassword -> "choose_password"
            | NewPassword -> "new_password"
            | ResetPassword -> "reset_password"
            | ForgotPassword -> "forgot_password"
            | InHouseClinicianEnrollment -> "in_house_clinician_enrollment"
            | FirstName -> "first_name"
            | LastName -> "last_name"
            | DateOfBirth -> "date_of_birth"
            | Username -> "username"
            | Email -> "email"
            | PhoneNumber -> "phone_number"
            | MobilePhoneNumber -> "mobile_phone_number"
            | HomePhoneNumber -> "home_phone_number"
            | PrimaryContact -> "primary_contact"
            | Address -> "address"
            | State -> "state"
            | VisitType -> "visit_type"
            | VisitTypeSelect -> "visit_type_select"
            | Facility -> "facility"
            | FacilitySelect -> "facility_select"
            | VisitReason -> "visit_reason"
            | Next -> "next"
            | Previous -> "previous"
            | Submit -> "submit"
            | Register -> "register"
            | Gender -> "gender"
            | Patient -> "patient"
            | Form -> "form"
            | SOAPNotes -> "soap_notes"
            | Date -> "date"
            | Time -> "time"
            | Location -> "location"
            | Flags -> "flags"
            | Severity -> "severity"
            | Unspecified -> "unspecified"
            | Mild -> "mild"
            | Moderate -> "moderate"
            | Suicidal -> "suicidal"
            | Points -> "points"


            //Forms
            | SelectPlaceholder -> "select_placeholder"


            | ProviderCharting -> "provider_charting"
            | Results -> "results"
            | Clinician -> "clinician"
            | ClinicalNotes -> "clinical_notes"
            | RecommendedCodes -> "recommended_codes"
            | DownloadPdf -> "download_pdf"
            | NoActiveRecommendedCodes -> "no_active_recommended_codes"
            | SuicidalWarning -> "suicidal_warning"
            | FunctionalPatient -> "functional_patient"
            | ScreeningZeroToFour -> "screening_zero_to_four"
            | ScreeningFiveToNine -> "screening_five_to_nine"
            | ScreeningTenToFourteen -> "screening_ten_to_fourteen"
            | ScreeningFifteenToNineteen -> "screening_fifteen_to_nineteen"
            | ScreeningTwentyOrGreater -> "screening_twenty_or_greater"

            | AppointmentDetails -> "appointment_details"
            | AppointmentDetailsSubtitle -> "appointment_details_subtitle"
            | InOffice -> "in_office"
            | TeleHealth -> "tele_health"

            | ThankYou -> "thank_you"
            | ThankYouMessage -> "thank_you_message"
            | ThankYouPleaseReturnDevice -> "thank_you_please_return_device"
            | ThankYouGetUpdatesMessage -> "thank_you_get_updates_message"
            | ThankYouDownloadApp -> "thank_you_download_app"

            | SignUpLetsGo -> "sign_up_lets_go"
            | SignUpPreferredLanguage -> "sign_up_preferred_language"
            | SignUpSubtitle -> "sign_up_subtitle"
            | SignUpSpokenLanguageStepSubtitle -> "sign_up_spoken_language_step_subtitle"
            | SignUpLoginInfo -> "sign_up_login_info"
            | SignUpLoginInfoSubtitle -> "sign_up_login_info_subtitle"
            | SignUpNameTitle -> "sign_up_name_title"
            | SignUpInsuranceTitle -> "sign_up_insurance_title"
            | SignUpAddressTitle -> "sign_up_address_title"
            | SignUpDobTitle -> "sign_up_dob_title"
            | SignUpGenderTitle -> "sign_up_gender_title"

            | SocialSecurityNumber -> "social_security_number"
            | InsuranceGroupNumber -> "insurance_group_number"
            | InsuranceCarrierName -> "insurance_carrier_name"
            | InsurancePlanName -> "insurance_plan_name"
            | InsuranceNumber -> "insurance_number"
            | InsuranceMemberId -> "insurance_member_id"
            | TelephoneNumber -> "telephone_number"
            | StreetAddress1 -> "street_address_1"
            | StreetAddress2 -> "street_address_2"
            | City -> "address_city"
            | ZipCode -> "address_zip_code"
            | ReferringReason -> "referring_reason"
            | FrontCard -> "front_card"
            | BackCard -> "back_card"

            | Avatar -> "avatar"

            | LandingContinueWithAntidote -> "landing_continue_with_antidote"

            | ForgotPasswordSendEmail -> "forgot_password_send_email"

            | Status -> "status"
            | StatusOnline -> "status_online"
            | StatusOffline -> "status_offline"
            | StatusDoNotDisturb -> "status_do_not_disturb"
            | StatusInCall -> "status_in_call"

            | HandoutModeEnter -> "handout_mode_enter"

            | NavbarIntakeHistory -> "navbar_intake_history"
            | NavbarPatientIntakeHistory -> "navbar_patient_intake_history"
            | NavbarProviderIntakeHistory -> "navbar_provider_intake_history"
            | NavbarReferralIntakeHistory -> "navbar_referral_intake_history"
            | NavbarVisitHistory -> "navbar_visity_history"
            | NavbarDeleteAccount -> "navbar_delete_account"

            //Site Map
            | PageHealixHome -> "page_healix_home"
            | PageUserProfile -> "page_user_profile"
            | PagePatientChart -> "page_patient_chart"

            | ProfileGreeting -> "profile_greeting"

            | AboutFindUsAppleStore -> "about_find_us_apple_store"
            | AboutReviewActivity -> "about_review_activity"
            | AboutScanQrToGetStarted -> "about_scan_qr_to_get_started"

            | January -> "january"
            | February -> "february"
            | March -> "march"
            | April -> "april"
            | May -> "may"
            | June -> "june"
            | July -> "july"
            | August -> "august"
            | September -> "september"
            | October -> "october"
            | November -> "november"
            | December -> "december"

            | Sunday -> "sunday"
            | Monday -> "monday"
            | Tuesday -> "tuesday"
            | Wednesday -> "wednesday"
            | Thursday -> "thursday"
            | Friday -> "friday"
            | Saturday -> "saturday"

            | Today -> "today"
            | Past -> "past"
            | Yesterday -> "yesterday"
            | Tomorrow -> "tomorrow"

            //Seattle7
            | DailyAngina -> "daily_angina"
            | WeeklyAngina -> "weekly_angina"
            | MonthlyAngina -> "monthly_angina"
            | NoAngina -> "no_angina"

            //CAT
            | CatLowRisk -> "cat_low_risk"
            | CatModerateRisk -> "cat_moderate_risk"
            | CatHighRisk -> "cat_high_risk"
            | CatVeryHighRisk -> "cat_very_high_risk"

            //DSM-5-TR
            //| ``Are you currently taking any prescription medications?`` -> "dsm5tr_q1"
            // | ``Do you find that you're frequently consuming more of a substance, or using it for longer periods than you originally planned?`` -> "dsm5tr_q2"
            // | DSM5TRQ3 -> "dsm5tr_q3"
            // | DSM5TRQ4 -> "dsm5tr_q4"
            // | DSM5TRQ5 -> "dsm5tr_q5"
            // | DSM5TRQ6 -> "dsm5tr_q6"
            // | DSM5TRQ7 -> "dsm5tr_q7"
            // | DSM5TRQ8 -> "dsm5tr_q8"
            // | DSM5TRQ9 -> "dsm5tr_q9"
            // | DSM5TRQ10 -> "dsm5tr_q10"
            // | DSM5TRQ11 -> "dsm5tr_q11"
            // | DSM5TRQ12 -> "dsm5tr_q12"
            // | DSM5TRQ13 -> "dsm5tr_q13"
            // | DSM5TRQ14 -> "dsm5tr_q14"

        key.ToLower()
