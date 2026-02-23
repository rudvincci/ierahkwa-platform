using Mamey.Binimoy.Requests;

namespace Mamey.Binimoy.Services;

public class ICPFinancialService : IICPFinancialService
{
    private readonly BinimoyOptions _options;

    public ICPFinancialService(BinimoyOptions options)
    {
        _options = options;
    }

    /// <inheritdoc />
    public async Task CreateRTPAsync(CreateRTPRequest request)// https://$root
    {
        request = new CreateRTPRequest
        {
            OrgId = "123456789",
            ChannelId = ChannelId.Mobile,
            SenderVId = "sampleUser@binimoy",
            ReceiverVId = "sampleUser2@binimoy",
            DeviceId = "012HGRTBHDGBKLBVB",
            MobileNumber = "8801864578345",
            Location = "23.7805,90.4267",
            IPAddress = "10.0.0.1",
            ReferenceNumber = "RTPIDTP20220201160918317",
            ReferenceNumberTransaction = "258542685255",
            RequestAmount = 5000.00m,
            Purpose = "Sample Purpose",
            CallFrom = "FIApp"
        };
        var url = $"{_options.Url}/financial/CreateRTP";
        // Generate the XML payload from the CreateRTPRequest object


        //// Prepare the HttpContent with the XML payload
        //var content = new StringContent(request.GenerateXmlPayload(), Encoding.UTF8, "application/xml");

        //// Send the HTTP POST request
        //HttpResponseMessage response = await _httpClient.PostAsync(url, content);

        //// Ensure the response was successful, otherwise throw an exception
        //response.EnsureSuccessStatusCode();

        //// Read and return the response content
        //return await response.Content.ReadAsStringAsync();

        throw new NotImplementedException();
    }
    

    /// <inheritdoc />
    public Task CreateRTPISOAsync()
    {

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task DisburseGovtFundsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task DisburseSalaryAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task GetIDTPFeeAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task GetPaymentAuthorizationAsync(GetPaymentAuthorizationRequest request)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task GetRTPListReceivedAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task GetRTPListSentAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task GetRTPsbyFIAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task GetRTPStatusAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task GetTransactionsbyFIAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task GetTransactionsbyUserAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task GetTransactionStatusAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task PayGovernmentDueISOAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task PayGovernmentDuesAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task PayUtilityBillsAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task PayUtilityBillsISOAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task RecordPaymentAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task SendRTPDeclinedResponseAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task SendRTPDeclinedResponseISOAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task TransferFundsAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task TransferFundsFItoFIAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task TransferFundsFItoFIISOAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task TransferFundsISOAsync()
    {
        throw new NotImplementedException();
    }
    /// <inheritdoc />
    public Task UpdateDisbursementStatusAsync()
    {
        throw new NotImplementedException();
    }
}
