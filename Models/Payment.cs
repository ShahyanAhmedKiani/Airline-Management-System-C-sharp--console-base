// =============================================================================
// File: Models/Payment.cs
// OOP: ENCAPSULATION — payment details hidden behind properties
//      INHERITANCE   — CreditPayment and DebitPayment extend Payment
//      POLYMORPHISM  — GetPaymentSummary() overridden in subclasses
// =============================================================================

namespace AirlineReservationSystem.Models
{
    /// <summary>
    /// Abstract base class for all payment methods.
    ///
    /// ✦ ENCAPSULATION: Sensitive fields (cvv, pin) kept private.
    /// ✦ INHERITANCE:   CreditPayment and DebitPayment both extend this.
    /// ✦ POLYMORPHISM:  GetPaymentSummary() is virtual → overridden per type.
    /// </summary>
    public abstract class Payment
    {
        // ── Private fields (Encapsulation) ────────────────────────────────────
        private int    _cvv;
        private string _pin = string.Empty;

        // ── Properties ────────────────────────────────────────────────────────
        public string BankName     { get; set; } = string.Empty;
        public string CardHolderFirstName { get; set; } = string.Empty;
        public string CardHolderLastName  { get; set; } = string.Empty;
        public string CardNumber   { get; set; } = string.Empty;
        public string ExpiryDate   { get; set; } = string.Empty;
        public decimal Amount      { get; set; }

        /// <summary>CVV — only readable within this assembly (semi-private).</summary>
        public int Cvv
        {
            get => _cvv;
            set => _cvv = (value < 100 || value > 9999)
                ? throw new ArgumentException("CVV must be 3–4 digits.")
                : value;
        }

        /// <summary>PIN — set-only from outside; never returned raw (Encapsulation).</summary>
        public void SetPin(string pin) => _pin = pin;
        public bool VerifyPin(string pin) => _pin == pin;

        // ── Abstract method (forces subclasses to declare payment type) ───────
        public abstract string PaymentType { get; }

        // ── Virtual summary (Polymorphism) ────────────────────────────────────
        public virtual string GetPaymentSummary()
            => $"{PaymentType} | Bank: {BankName} | Card: ****{CardNumber[^4..]} | Amount: PKR {Amount:N0}";
    }

    // =========================================================================
    // ✦ INHERITANCE: CreditPayment extends Payment
    // =========================================================================

    /// <summary>Credit card payment — inherits all Payment members.</summary>
    public sealed class CreditPayment : Payment
    {
        public override string PaymentType => "Credit Card";

        public override string GetPaymentSummary()
            => $"[CREDIT] {base.GetPaymentSummary()}";
    }

    // =========================================================================
    // ✦ INHERITANCE: DebitPayment extends Payment
    // =========================================================================

    /// <summary>Debit card payment — inherits all Payment members.</summary>
    public sealed class DebitPayment : Payment
    {
        public override string PaymentType => "Debit Card";

        public override string GetPaymentSummary()
            => $"[DEBIT]  {base.GetPaymentSummary()}";
    }
}
