namespace FluentACS.ManagementService
{
    public enum TokenType
    {
        SWT,
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is converted to a string that is the same name passed to the endpoint.")]
        SAML_1_1,
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is converted to a string that is the same name passed to the endpoint.")]
        SAML_2_0
    }
}