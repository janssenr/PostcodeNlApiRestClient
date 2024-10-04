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
        <h1>Postcode.nl API REST client example</h1>
        <ul>
            <li><a href="https://api.postcode.nl/documentation">Postcode.nl API documentation</a></li>
        </ul>
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
        <asp:Button ID="btnAddressSend" runat="server" Text="Send" OnClick="btnAddressSend_Click" />
        <asp:Panel ID="pnlResult" runat="server">
            <hr />
            <h2>Validated address</h2>
            <pre><asp:Literal ID="lStreet" runat="server"></asp:Literal>&nbsp;<asp:Literal ID="lHouseNumber" runat="server"></asp:Literal>&nbsp;<asp:Literal ID="lHouseNumberAddition" runat="server"></asp:Literal><br/><asp:Literal ID="lPostcode" runat="server"></asp:Literal>&nbsp;<asp:Literal ID="lCity" runat="server"></asp:Literal></pre>
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
        <h2>Example address data:</h2>
        <h3>Existing postcode, with no housenumber addition</h3>
        <p>
            Postcode: `2012ES`<br />
            Housenumber: `30`<br />
            Housenumber addition: ``<br />
        </p>
        <h3>Existing postcode, with only one possible housenumber addition</h3>
        <p>
            Postcode: `1011AE`<br />
            Housenumber: `36`<br />
            Housenumber addition: `B`<br />
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
        <h3>Result showing difference between street and streetNen</h3>
        <p>
            Postcode: `1011DG`<br />
            Housenumber: `2`<br />
            Housenumber addition: ``<br />
        </p>
    </form>
</body>
</html>
