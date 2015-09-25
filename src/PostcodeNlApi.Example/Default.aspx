<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Postcode.nl API REST client example</title>
    <style>
        fieldset {
            margin: 8px 0;
            padding: 16px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <fieldset>
            <legend>Authentication</legend>
            API url:<br />
            <asp:TextBox ID="txtApiUrl" runat="server"></asp:TextBox><br />
            API key:<br />
            <asp:TextBox ID="txtKey" runat="server"></asp:TextBox><br />
            API secret:<br />
            <asp:TextBox ID="txtSecret" runat="server"></asp:TextBox><br />
        </fieldset>
        <br />
        <b>Select service:</b><br />
        <asp:DropDownList ID="ddlService" runat="server">
            <Items>
                <asp:ListItem Value="Address" Text="Postcode.nl Address API: Address lookup"></asp:ListItem>
                <asp:ListItem Value="Signal" Text="Postcode.nl Signal API: Check"></asp:ListItem>
            </Items>
        </asp:DropDownList>
        <asp:Button ID="btnSelect" runat="server" Text="Select" OnClick="btnSelect_Click" />
        <br />
        <asp:Panel ID="pnlAddress" runat="server" GroupingText="">
            <fieldset>
                <legend>Input parameters</legend>
                Postcode:<br />
                <asp:TextBox ID="txtPostcode" runat="server"></asp:TextBox><br />
                House number:<br />
                <asp:TextBox ID="txtHouseNumber" runat="server"></asp:TextBox><br />
                House number addition:<br />
                <asp:TextBox ID="txtHouseNumberAddition" runat="server"></asp:TextBox><br />
                <asp:CheckBox ID="chkValidateHouseNumberAddition" runat="server" Text="Strictly validate addition" /><br />
            </fieldset>
            <br />
            <asp:Button ID="btnAddressSend" runat="server" Text="Send" OnClick="btnAddressSend_Click" /><asp:CheckBox ID="chkAddressShowRawRequestResponse" runat="server" Text="Show raw HTTP request and response" /><br />
        </asp:Panel>
        <asp:Panel ID="pnlSignal" runat="server">
            <fieldset>
                <legend>Input parameters</legend>
                <fieldset>
                    <legend>Customer</legend>
                    First name:<br />
                    <asp:TextBox ID="txtCustomerFirstName" runat="server"></asp:TextBox><br />
                    Last name:<br />
                    <asp:TextBox ID="txtCustomerLastName" runat="server"></asp:TextBox><br />
                    Birth date:<br />
                    <asp:TextBox ID="txtCustomerBirthDate" runat="server"></asp:TextBox><br />
                    Email:<br />
                    <asp:TextBox ID="txtCustomerEmail" runat="server"></asp:TextBox><br />
                    Email domain:<br />
                    <asp:TextBox ID="txtCustomerEmailDomain" runat="server"></asp:TextBox><br />
                    Phone number:<br />
                    <asp:TextBox ID="txtCustomerPhoneNumber" runat="server"></asp:TextBox><br />
                    Bank number:<br />
                    <asp:TextBox ID="txtCustomerBankNumber" runat="server"></asp:TextBox><br />
                    Site:<br />
                    <asp:TextBox ID="txtCustomerSite" runat="server"></asp:TextBox><br />
                    Internal ID:<br />
                    <asp:TextBox ID="txtCustomerInternalId" runat="server"></asp:TextBox><br />
                    <fieldset>
                        <legend>Address</legend>
                        Postcode:<br />
                        <asp:TextBox ID="txtCustomerAddressPostcode" runat="server"></asp:TextBox><br />
                        House number:<br />
                        <asp:TextBox ID="txtCustomerAddressHouseNumber" runat="server"></asp:TextBox><br />
                        House number addition:<br />
                        <asp:TextBox ID="txtCustomerAddressHouseNumberAddition" runat="server"></asp:TextBox><br />
                        Street:<br />
                        <asp:TextBox ID="txtCustomerAddressStreet" runat="server"></asp:TextBox><br />
                        City:<br />
                        <asp:TextBox ID="txtCustomerAddressCity" runat="server"></asp:TextBox><br />
                        Region:<br />
                        <asp:TextBox ID="txtCustomerAddressRegion" runat="server"></asp:TextBox><br />
                        Country:<br />
                        <asp:DropDownList ID="ddlCustomerAddressCountry" runat="server">
                            <Items>
                                <asp:ListItem Value="" Text="- Unknown -"></asp:ListItem>
                                <asp:ListItem Value="NL" Text="Netherlands"></asp:ListItem>
                                <asp:ListItem Value="" Text="Not Netherlands"></asp:ListItem>
                            </Items>
                        </asp:DropDownList><br />
                    </fieldset>
                    <fieldset>
                        <legend>Company</legend>
                        Name:<br />
                        <asp:TextBox ID="txtCustomerCompanyName" runat="server"></asp:TextBox><br />
                        Government ID:<br />
                        <asp:TextBox ID="txtCustomerCompanyGovernmentId" runat="server"></asp:TextBox><br />
                        Country:<br />
                        <asp:DropDownList ID="ddlCustomerCompanyCountry" runat="server">
                            <Items>
                                <asp:ListItem Value="" Text="- Unknown -"></asp:ListItem>
                                <asp:ListItem Value="NL" Text="Netherlands"></asp:ListItem>
                                <asp:ListItem Value="" Text="Not Netherlands"></asp:ListItem>
                            </Items>
                        </asp:DropDownList><br />
                    </fieldset>
                </fieldset>
                <fieldset>
                    <legend>Access</legend>
                    IP address:<br />
                    <asp:TextBox ID="txtAccessIpAddress" runat="server"></asp:TextBox><br />
                    Additional IP addresses:<br />
                    <asp:TextBox ID="txtAccessAdditionalUpAddresses" runat="server" TextMode="MultiLine"></asp:TextBox>(newline separated)<br />
                    Session ID:<br />
                    <asp:TextBox ID="txtAccessSessionId" runat="server"></asp:TextBox><br />
                    Time:<br />
                    <asp:TextBox ID="txtAccessTime" runat="server"></asp:TextBox><br />
                    <fieldset>
                        <legend>Browser</legend>
                        User agent:<br />
                        <asp:TextBox ID="txtAccessBrowserUserAgent" runat="server"></asp:TextBox><br />
                        Accept language:<br />
                        <asp:TextBox ID="txtAccessBrowserAcceptLanguage" runat="server"></asp:TextBox><br />
                    </fieldset>
                </fieldset>
                <fieldset>
                    <legend>Transaction</legend>
                    Internal ID:<br />
                    <asp:TextBox ID="txtTransactionInternalId" runat="server"></asp:TextBox><br />
                    Status:<br />
                    <asp:DropDownList ID="ddlTransactionStatus" runat="server">
                        <Items>
                            <asp:ListItem Value="" Text="- Not set -"></asp:ListItem>
                        </Items>
                    </asp:DropDownList><br />
                    Cost:<br />
                    <asp:TextBox ID="txtTransactionCost" runat="server"></asp:TextBox><br />
                    Cost Currency:<br />
                    <asp:TextBox ID="txtTransactionCostCurrency" runat="server"></asp:TextBox><br />
                    Payment type:<br />
                    <asp:TextBox ID="txtTransactionPaymentType" runat="server"></asp:TextBox><br />
                    Weight:<br />
                    <asp:TextBox ID="txtTransactionWeight" runat="server"></asp:TextBox><br />
                    <fieldset>
                        <legend>Delivery Address</legend>
                        Postcode:<br />
                        <asp:TextBox ID="txtTransactionDeliveryAddressPostcode" runat="server"></asp:TextBox><br />
                        House number:<br />
                        <asp:TextBox ID="txtTransactionDeliveryAddressHouseNumber" runat="server"></asp:TextBox><br />
                        House number addition:<br />
                        <asp:TextBox ID="txtTransactionDeliveryAddressHouseNumberAddition" runat="server"></asp:TextBox><br />
                        Street:<br />
                        <asp:TextBox ID="txtTransactionDeliveryAddressStreet" runat="server"></asp:TextBox><br />
                        City:<br />
                        <asp:TextBox ID="txtTransactionDeliveryAddressCity" runat="server"></asp:TextBox><br />
                        Region:<br />
                        <asp:TextBox ID="txtTransactionDeliveryAddressRegion" runat="server"></asp:TextBox><br />
                        Country:<br />
                        <asp:DropDownList ID="ddlTransactionDeliveryAddressCountry" runat="server">
                            <Items>
                                <asp:ListItem Value="" Text="- Unknown -"></asp:ListItem>
                                <asp:ListItem Value="NL" Text="Netherlands"></asp:ListItem>
                                <asp:ListItem Value="" Text="Not Netherlands"></asp:ListItem>
                            </Items>
                        </asp:DropDownList><br />
                    </fieldset>
                </fieldset>
                <fieldset>
                    <legend>Config</legend>
                    Select Services:<br />
                    <asp:TextBox ID="txtConfigSelectServices" runat="server" TextMode="MultiLine"></asp:TextBox>(newline separated)<br />
                    Exclude Services:<br />
                    <asp:TextBox ID="txtConfigExcludeServices" runat="server" TextMode="MultiLine"></asp:TextBox>(newline separated)<br />
                    Select Result Types:<br />
                    <asp:TextBox ID="txtConfigSelectTypes" runat="server" TextMode="MultiLine"></asp:TextBox>(newline separated)<br />
                    Exclude Result Types:<br />
                    <asp:TextBox ID="txtConfigExcludeTypes" runat="server" TextMode="MultiLine"></asp:TextBox>(newline separated)<br />
                </fieldset>
            </fieldset>
            <br />
            <asp:Button ID="btnSignalSend" runat="server" Text="Send" OnClick="btnSignalSend_Click" /><asp:CheckBox ID="chkSignalShowRawRequestResponse" runat="server" Text="Show raw HTTP request and response" /><br />
        </asp:Panel>
        <asp:Panel ID="pnlResult" runat="server">
            <asp:Panel ID="pnlAddressResponse" runat="server">
                <hr />
                <h2>Validated address</h2>
                <pre><asp:Literal ID="lStreet" runat="server"></asp:Literal>&nbsp;<asp:Literal ID="lHouseNumber" runat="server"></asp:Literal>&nbsp;<asp:Literal ID="lHouseNumberAddition" runat="server"></asp:Literal><br/><asp:Literal ID="lPostcode" runat="server"></asp:Literal>&nbsp;<asp:Literal ID="lCity" runat="server"></asp:Literal></pre>
            </asp:Panel>
            <asp:Panel ID="pnlSignalResponse" runat="server">
                <hr />
                <h2>Signal check response</h2>
                <asp:Literal ID="lSignalCount" runat="server"></asp:Literal>
                signal(s) reported:
                <asp:HyperLink ID="hplReportPdfUrl" runat="server" Target="_blank">PDF report</asp:HyperLink><br />
            </asp:Panel>
            <h3>Response data:</h3>
            <pre><asp:Literal ID="lResult" runat="server"></asp:Literal></pre>
        </asp:Panel>
        <asp:Panel ID="pnlError" runat="server">
            <hr />
            <h2>
                <asp:Literal ID="lType" runat="server"></asp:Literal>
            </h2>
            <asp:Literal ID="lMessage" runat="server"></asp:Literal><br />
            (class: <em>
                <asp:Literal ID="lClass" runat="server"></asp:Literal></em>)<br />
        </asp:Panel>
        <asp:Panel ID="pnlTimeTaken" runat="server">
            <h4>Time taken:</h4>
            <p>
                <asp:Literal ID="lTimeTaken" runat="server"></asp:Literal>
                sec
            </p>
        </asp:Panel>
        <asp:Panel ID="pnlRawRequestResponse" runat="server">
            <h2>Raw HTTP request and response</h2>
            <fieldset>
                <legend>Raw HTTP request headers</legend>
                <pre><asp:Literal ID="lRequest" runat="server"></asp:Literal></pre>
            </fieldset>
            <fieldset>
                <legend>Raw HTTP response headers + body</legend>
                <pre><asp:Literal ID="lResponse" runat="server"></asp:Literal></pre>
            </fieldset>
        </asp:Panel>
        <br />
        <hr />
        <br />
        <h2>Example data:</h2>
        <h3>Existing postcode, with no housenumber addition</h3>
        <p>
            Postcode: `2012ES`<br />
            Housenumber: `30`<br />
            Housenumber addition: ``<br />
        </p>
        <h3>Existing postcode, with only one possible housenumber addition</h3>
        <p>
            Postcode: `2011DW`<br />
            Housenumber: `8`<br />
            Housenumber addition: `RD`<br />
        </p>
        <h3>Existing postcode, with multiple housenumber additions, incorrect addition</h3>
        <p>
            Postcode: `2011DW`<br />
            Housenumber: `9`<br />
            Housenumber addition: `ZZZ`<br />
        </p>
        <h3>Non-existing postcode</h3>
        <p>
            Postcode: `1234ZZ`<br />
            Housenumber: `1234`<br />
            Housenumber addition: ``<br />
        </p>
    </form>
</body>
</html>
