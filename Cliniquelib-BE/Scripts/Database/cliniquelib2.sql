
-- =============================================================================
-- Clinic/EHR Mega Schema (PostgreSQL)
-- Scope: Multi-tenant clinics, scheduling, telemed, EHR, labs, imaging,
-- notifications (email/SMS/push), billing/insurance, audit/RLS.
-- =============================================================================

-- Extensions
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE EXTENSION IF NOT EXISTS "citext";

-- =============================================================================
-- 1) Tenancy, orgs, users, roles (RBAC)
-- =============================================================================
CREATE TABLE organizations (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT NOT NULL,
  legal_name TEXT,
  country_code CHAR(2),
  timezone TEXT NOT NULL,
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE clinics (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  address JSONB,
  phone TEXT,
  email CITEXT,
  timezone TEXT NOT NULL,
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE users (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  organization_id UUID REFERENCES organizations(id) ON DELETE CASCADE,
  email CITEXT UNIQUE NOT NULL,
  phone TEXT,
  password_hash TEXT,
  first_name TEXT NOT NULL,
  last_name TEXT NOT NULL,
  dob DATE,
  sex TEXT,
  locale TEXT DEFAULT 'en',
  is_active BOOLEAN NOT NULL DEFAULT true,
  mfa_methods JSONB,
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE roles (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT UNIQUE NOT NULL,
  description TEXT
);

CREATE TABLE refresh_tokens (
  token UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  expires_at TIMESTAMPTZ NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE user_roles (
  user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  clinic_id UUID REFERENCES clinics(id) ON DELETE CASCADE,
  role_id UUID NOT NULL REFERENCES roles(id) ON DELETE RESTRICT,
  PRIMARY KEY (user_id, clinic_id, role_id)
);

-- Practitioners (providers) & staff profiles
CREATE TABLE practitioners (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID NOT NULL UNIQUE REFERENCES users(id) ON DELETE CASCADE,
  clinic_id UUID NOT NULL REFERENCES clinics(id) ON DELETE CASCADE,
  npi TEXT,
  specialties TEXT[],
  license_number TEXT,
  calendar_prefs JSONB,
  meta JSONB
);

CREATE TABLE staff (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID NOT NULL UNIQUE REFERENCES users(id) ON DELETE CASCADE,
  clinic_id UUID NOT NULL REFERENCES clinics(id) ON DELETE CASCADE,
  position TEXT,
  meta JSONB
);

-- Patients are also users (patient portal)
CREATE TABLE patients (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID UNIQUE REFERENCES users(id) ON DELETE SET NULL,
  clinic_id UUID NOT NULL REFERENCES clinics(id) ON DELETE CASCADE,
  mrn TEXT UNIQUE,
  identifiers JSONB,
  address JSONB,
  emergency_contacts JSONB,
  primary_practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  flags JSONB,
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Consents & privacy
CREATE TABLE consents (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  clinic_id UUID NOT NULL REFERENCES clinics(id) ON DELETE CASCADE,
  type TEXT NOT NULL,
  scope JSONB,
  granted_at TIMESTAMPTZ NOT NULL,
  revoked_at TIMESTAMPTZ,
  meta JSONB
);

-- =============================================================================
-- 2) Coding systems & codeable concepts (SNOMED/LOINC/ICD/etc.)
-- =============================================================================
CREATE TABLE code_systems (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  code TEXT UNIQUE NOT NULL,
  name TEXT NOT NULL,
  version TEXT,
  meta JSONB
);

CREATE TABLE codes (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  system_id UUID NOT NULL REFERENCES code_systems(id) ON DELETE CASCADE,
  code TEXT NOT NULL,
  display TEXT,
  properties JSONB,
  UNIQUE (system_id, code)
);

CREATE TABLE coded_terms (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  text TEXT,
  coding JSONB
);

-- =============================================================================
-- 3) Scheduling: locations, rooms, availability (RRULE), appointments
-- =============================================================================
CREATE TABLE locations (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  clinic_id UUID NOT NULL REFERENCES clinics(id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  type TEXT,
  address JSONB,
  geo JSONB,
  meta JSONB
);

CREATE TABLE schedules (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  practitioner_id UUID REFERENCES practitioners(id) ON DELETE CASCADE,
  location_id UUID REFERENCES locations(id) ON DELETE SET NULL,
  timezone TEXT NOT NULL,
  rrule TEXT NOT NULL,
  start_date DATE NOT NULL,
  end_date DATE,
  exdates TIMESTAMPTZ[],
  capacity INT NOT NULL DEFAULT 1,
  meta JSONB
);

CREATE TABLE appointment_slots (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  schedule_id UUID NOT NULL REFERENCES schedules(id) ON DELETE CASCADE,
  start_at TIMESTAMPTZ NOT NULL,
  end_at TIMESTAMPTZ NOT NULL,
  capacity INT NOT NULL DEFAULT 1,
  booked_count INT NOT NULL DEFAULT 0,
  UNIQUE (schedule_id, start_at)
);

CREATE TABLE appointments (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  clinic_id UUID NOT NULL REFERENCES clinics(id) ON DELETE CASCADE,
  patient_id UUID REFERENCES patients(id) ON DELETE SET NULL,
  practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  slot_id UUID REFERENCES appointment_slots(id) ON DELETE SET NULL,
  type TEXT NOT NULL,
  status TEXT NOT NULL DEFAULT 'reserved',
  reason TEXT,
  coded_reason UUID REFERENCES coded_terms(id),
  location_id UUID REFERENCES locations(id) ON DELETE SET NULL,
  start_at TIMESTAMPTZ NOT NULL,
  end_at TIMESTAMPTZ NOT NULL,
  source TEXT,
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE waitlist_entries (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  clinic_id UUID NOT NULL REFERENCES clinics(id) ON DELETE CASCADE,
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  desired_from TIMESTAMPTZ,
  desired_to TIMESTAMPTZ,
  notes TEXT,
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- =============================================================================
-- 4) Teleconsultation (WebRTC) and messaging
-- =============================================================================
CREATE TABLE telemed_sessions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  appointment_id UUID NOT NULL UNIQUE REFERENCES appointments(id) ON DELETE CASCADE,
  practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  patient_id UUID REFERENCES patients(id) ON DELETE SET NULL,
  room_id TEXT NOT NULL,
  join_tokens JSONB,
  status TEXT NOT NULL DEFAULT 'scheduled',
  started_at TIMESTAMPTZ,
  ended_at TIMESTAMPTZ,
  recording_file_id UUID,
  meta JSONB
);

CREATE TABLE messages (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  telemed_session_id UUID REFERENCES telemed_sessions(id) ON DELETE CASCADE,
  sender_user_id UUID REFERENCES users(id) ON DELETE SET NULL,
  channel TEXT NOT NULL DEFAULT 'chat',
  body TEXT,
  attachments JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- =============================================================================
-- 5) EHR core: encounters, problems, allergies, meds, immunizations, vitals
-- =============================================================================
CREATE TABLE encounters (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  clinic_id UUID NOT NULL REFERENCES clinics(id) ON DELETE CASCADE,
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  appointment_id UUID REFERENCES appointments(id) ON DELETE SET NULL,
  type TEXT,
  start_at TIMESTAMPTZ,
  end_at TIMESTAMPTZ,
  reason TEXT,
  coded_reason UUID REFERENCES coded_terms(id),
  notes JSONB,
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE conditions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  encounter_id UUID REFERENCES encounters(id) ON DELETE SET NULL,
  code UUID REFERENCES coded_terms(id),
  onset TIMESTAMPTZ,
  abatement TIMESTAMPTZ,
  clinical_status TEXT,
  verification_status TEXT,
  notes TEXT,
  meta JSONB
);

CREATE TABLE allergies (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  substance UUID REFERENCES coded_terms(id),
  reaction TEXT,
  severity TEXT,
  status TEXT DEFAULT 'active',
  notes TEXT,
  meta JSONB
);

CREATE TABLE medications (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  code UUID REFERENCES coded_terms(id),
  dose TEXT,
  route TEXT,
  frequency TEXT,
  start_date DATE,
  end_date DATE,
  status TEXT,
  notes TEXT,
  meta JSONB
);

CREATE TABLE prescriptions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  encounter_id UUID REFERENCES encounters(id) ON DELETE SET NULL,
  items JSONB NOT NULL,
  signed_at TIMESTAMPTZ,
  pdf_file_id UUID,
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE immunizations (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  code UUID REFERENCES coded_terms(id),
  date_administered DATE,
  lot_number TEXT,
  site TEXT,
  route TEXT,
  notes TEXT,
  meta JSONB
);

CREATE TABLE observations (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  encounter_id UUID REFERENCES encounters(id) ON DELETE SET NULL,
  code UUID REFERENCES coded_terms(id),
  value TEXT,
  value_json JSONB,
  unit TEXT,
  effective_at TIMESTAMPTZ,
  status TEXT DEFAULT 'final',
  meta JSONB
);

CREATE TABLE procedures (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  encounter_id UUID REFERENCES encounters(id) ON DELETE SET NULL,
  code UUID REFERENCES coded_terms(id),
  performed_at TIMESTAMPTZ,
  notes TEXT,
  meta JSONB
);

CREATE TABLE file_objects (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  bucket TEXT NOT NULL,
  object_key TEXT NOT NULL,
  mime TEXT NOT NULL,
  size BIGINT NOT NULL,
  checksum TEXT,
  created_by UUID REFERENCES users(id),
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  UNIQUE (bucket, object_key)
);

CREATE TABLE documents (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID REFERENCES patients(id) ON DELETE SET NULL,
  type UUID REFERENCES coded_terms(id),
  title TEXT,
  file_id UUID REFERENCES file_objects(id) ON DELETE SET NULL,
  created_by UUID REFERENCES users(id),
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  meta JSONB
);

-- =============================================================================
-- 6) Orders: Labs & Imaging, Diagnostic Reports
-- =============================================================================
CREATE TABLE lab_orders (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  encounter_id UUID REFERENCES encounters(id) ON DELETE SET NULL,
  practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  tests JSONB NOT NULL,
  status TEXT NOT NULL DEFAULT 'ordered',
  ordered_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  result_report_id UUID,
  meta JSONB
);

CREATE TABLE imaging_orders (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  encounter_id UUID REFERENCES encounters(id) ON DELETE SET NULL,
  practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  modality TEXT,
  indication TEXT,
  status TEXT NOT NULL DEFAULT 'ordered',
  ordered_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  result_report_id UUID,
  meta JSONB
);

CREATE TABLE diagnostic_reports (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  encounter_id UUID REFERENCES encounters(id) ON DELETE SET NULL,
  type UUID REFERENCES coded_terms(id),
  conclusion TEXT,
  codes JSONB,
  results JSONB,
  files UUID[],
  issued_at TIMESTAMPTZ,
  status TEXT DEFAULT 'final',
  meta JSONB
);

CREATE TABLE imaging_studies (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  imaging_order_id UUID REFERENCES imaging_orders(id) ON DELETE SET NULL,
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  modality TEXT,
  accession_number TEXT,
  dicom_study_uid TEXT,
  dicom_series JSONB,
  files UUID[],
  started_at TIMESTAMPTZ,
  ended_at TIMESTAMPTZ,
  meta JSONB
);

-- =============================================================================
-- 7) Notifications
-- =============================================================================
CREATE TABLE notification_templates (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
  channel TEXT NOT NULL,
  key TEXT NOT NULL,
  subject TEXT,
  body_md TEXT NOT NULL,
  locale TEXT NOT NULL DEFAULT 'en',
  meta JSONB,
  UNIQUE (organization_id, channel, key, locale)
);

CREATE TABLE notifications (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
  clinic_id UUID REFERENCES clinics(id) ON DELETE SET NULL,
  template_id UUID REFERENCES notification_templates(id) ON DELETE SET NULL,
  channel TEXT NOT NULL,
  to_ref JSONB NOT NULL,
  payload_json JSONB NOT NULL,
  status TEXT NOT NULL DEFAULT 'pending',
  provider_msg_id TEXT,
  last_error TEXT,
  attempts INT NOT NULL DEFAULT 0,
  scheduled_at TIMESTAMPTZ,
  sent_at TIMESTAMPTZ,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- =============================================================================
-- 8) Billing, invoices, payments, insurance coverage
-- =============================================================================
CREATE TABLE payers (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  organization_id UUID REFERENCES organizations(id) ON DELETE SET NULL,
  name TEXT NOT NULL,
  country CHAR(2),
  payer_type TEXT,
  meta JSONB
);

CREATE TABLE coverages (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  payer_id UUID REFERENCES payers(id) ON DELETE SET NULL,
  plan TEXT,
  member_no TEXT,
  valid_from DATE,
  valid_to DATE,
  status TEXT NOT NULL DEFAULT 'active',
  meta JSONB
);

CREATE TABLE invoices (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  clinic_id UUID NOT NULL REFERENCES clinics(id) ON DELETE CASCADE,
  patient_id UUID REFERENCES patients(id) ON DELETE SET NULL,
  currency CHAR(3) NOT NULL,
  total_cents BIGINT NOT NULL DEFAULT 0,
  status TEXT NOT NULL DEFAULT 'draft',
  due_at TIMESTAMPTZ,
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE invoice_items (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  invoice_id UUID NOT NULL REFERENCES invoices(id) ON DELETE CASCADE,
  code UUID REFERENCES coded_terms(id),
  description TEXT,
  quantity NUMERIC(12,2) NOT NULL DEFAULT 1,
  unit_price_cents BIGINT NOT NULL DEFAULT 0,
  tax_cents BIGINT NOT NULL DEFAULT 0,
  meta JSONB
);

CREATE TABLE payments (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  invoice_id UUID NOT NULL REFERENCES invoices(id) ON DELETE CASCADE,
  amount_cents BIGINT NOT NULL,
  method TEXT,
  provider_ref TEXT,
  status TEXT NOT NULL DEFAULT 'pending',
  paid_at TIMESTAMPTZ,
  meta JSONB
);

-- =============================================================================
-- 9) Referrals & care plans
-- =============================================================================
CREATE TABLE referrals (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  from_practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  to_practitioner_id UUID REFERENCES practitioners(id) ON DELETE SET NULL,
  reason TEXT,
  coded_reason UUID REFERENCES coded_terms(id),
  notes TEXT,
  status TEXT DEFAULT 'requested',
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  meta JSONB
);

CREATE TABLE care_plans (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  patient_id UUID NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
  goals JSONB,
  activities JSONB,
  status TEXT DEFAULT 'active',
  meta JSONB,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- =============================================================================
-- 10) Auditing (append-only)
-- =============================================================================
CREATE TABLE audit_logs (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  organization_id UUID REFERENCES organizations(id) ON DELETE SET NULL,
  clinic_id UUID REFERENCES clinics(id) ON DELETE SET NULL,
  actor_user_id UUID REFERENCES users(id) ON DELETE SET NULL,
  action TEXT NOT NULL,
  entity TEXT NOT NULL,
  entity_id UUID,
  before_json JSONB,
  after_json JSONB,
  ip INET,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

ALTER TABLE audit_logs
ALTER COLUMN before_json SET NOT NULL,
ALTER COLUMN after_json SET NOT NULL;
REVOKE UPDATE, DELETE ON audit_logs FROM PUBLIC;

CREATE TABLE access_logs (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  actor_user_id UUID REFERENCES users(id) ON DELETE SET NULL,
  patient_id UUID REFERENCES patients(id) ON DELETE SET NULL,
  purpose TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  meta JSONB
);

-- =============================================================================
-- 11) Events bus
-- =============================================================================
CREATE TABLE domain_events (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name TEXT NOT NULL,
  occurred_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  aggregate_type TEXT NOT NULL,
  aggregate_id UUID,
  payload JSONB NOT NULL,
  published BOOLEAN NOT NULL DEFAULT false,
  meta JSONB
);

-- =============================================================================
-- 12) RLS scaffolding
-- =============================================================================
ALTER TABLE clinics ENABLE ROW LEVEL SECURITY;
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE patients ENABLE ROW LEVEL SECURITY;
ALTER TABLE appointments ENABLE ROW LEVEL SECURITY;
ALTER TABLE encounters ENABLE ROW LEVEL SECURITY;
ALTER TABLE invoices ENABLE ROW LEVEL SECURITY;

DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_settings WHERE name = 'app.current_org_id') THEN
    PERFORM set_config('app.current_org_id', '00000000-0000-0000-0000-000000000000', false);
  END IF;
  IF NOT EXISTS (SELECT 1 FROM pg_settings WHERE name = 'app.current_clinic_id') THEN
    PERFORM set_config('app.current_clinic_id', '00000000-0000-0000-0000-000000000000', false);
  END IF;
END;
$$;

CREATE POLICY clinics_isolation ON clinics
USING (organization_id::text = current_setting('app.current_org_id', true));

CREATE POLICY users_isolation ON users
USING (organization_id::text = current_setting('app.current_org_id', true));

CREATE POLICY patients_isolation ON patients
USING (clinic_id::text = current_setting('app.current_clinic_id', true));

CREATE POLICY appointments_isolation ON appointments
USING (clinic_id::text = current_setting('app.current_clinic_id', true));

CREATE POLICY encounters_isolation ON encounters
USING (clinic_id::text = current_setting('app.current_clinic_id', true));

CREATE POLICY invoices_isolation ON invoices
USING (clinic_id::text = current_setting('app.current_clinic_id', true));

-- =============================================================================
-- 13) Useful indexes
-- =============================================================================
CREATE INDEX idx_patients_clinic_mrn ON patients (clinic_id, mrn);
CREATE INDEX idx_appts_practitioner_time ON appointments (practitioner_id, start_at);
CREATE INDEX idx_appts_patient_time ON appointments (patient_id, start_at);
CREATE INDEX idx_obs_patient_time ON observations (patient_id, effective_at);
CREATE INDEX idx_diag_reports_patient_time ON diagnostic_reports (patient_id, issued_at);
CREATE INDEX idx_notifications_status_sched ON notifications (status, scheduled_at);
CREATE INDEX idx_domain_events_published ON domain_events (published);

-- =============================================================================
-- 14) Minimal triggers
-- =============================================================================
CREATE OR REPLACE FUNCTION trg_appointments_booked_count()
RETURNS trigger AS $$
BEGIN
  IF (TG_OP = 'INSERT') THEN
    IF NEW.slot_id IS NOT NULL AND NEW.status IN ('reserved', 'confirmed', 'in_progress') THEN
      UPDATE appointment_slots SET booked_count = booked_count + 1 WHERE id = NEW.slot_id;
    END IF;
  ELSIF (TG_OP = 'UPDATE') THEN
    IF OLD.slot_id IS DISTINCT FROM NEW.slot_id THEN
      IF OLD.slot_id IS NOT NULL AND OLD.status IN ('reserved', 'confirmed', 'in_progress') THEN
        UPDATE appointment_slots SET booked_count = GREATEST(booked_count - 1, 0) WHERE id = OLD.slot_id;
      END IF;
      IF NEW.slot_id IS NOT NULL AND NEW.status IN ('reserved', 'confirmed', 'in_progress') THEN
        UPDATE appointment_slots SET booked_count = booked_count + 1 WHERE id = NEW.slot_id;
      END IF;
    ELSIF OLD.status <> NEW.status THEN
      IF NEW.status IN ('cancelled', 'no_show', 'completed') AND OLD.status IN ('reserved', 'confirmed', 'in_progress') AND NEW.slot_id IS NOT NULL THEN
        UPDATE appointment_slots SET booked_count = GREATEST(booked_count - 1, 0) WHERE id = NEW.slot_id;
      ELSIF NEW.status IN ('reserved', 'confirmed', 'in_progress') AND OLD.status NOT IN ('reserved', 'confirmed', 'in_progress') AND NEW.slot_id IS NOT NULL THEN
        UPDATE appointment_slots SET booked_count = booked_count + 1 WHERE id = NEW.slot_id;
      END IF;
    END IF;
  ELSIF (TG_OP = 'DELETE') THEN
    IF OLD.slot_id IS NOT NULL AND OLD.status IN ('reserved', 'confirmed', 'in_progress') THEN
      UPDATE appointment_slots SET booked_count = GREATEST(booked_count - 1, 0) WHERE id = OLD.slot_id;
    END IF;
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_appt_slot_book ON appointments;
CREATE TRIGGER trg_appt_slot_book
AFTER INSERT OR UPDATE OR DELETE ON appointments
FOR EACH ROW EXECUTE FUNCTION trg_appointments_booked_count();
