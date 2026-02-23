
$repos = ("Mamey", "Mamey", "Mamey.Accounts", "Mamey.Bank.Accounts", 
"Mamey.Bank.ApiGateway", "Mamey.Bank.Beneficiaries", "Mamey.Bank.Branches", 
"Mamey.Bank.Cards", "Mamey.Bank.CentralBanks", "Mamey.Bank.Correspondents", 
"Mamey.Bank.Deposits", "Mamey.Bank.Documents", "Mamey.Bank.HelpDesk", "Mamey.Bank.Ktt", 
"Mamey.Bank.Ktt.Parser", "Mamey.Bank.Ledgers",  "Mamey.Bank.Licenses",
"Mamey.Bank.Loans", "Mamey.Bank.Members", "Mamey.Bank.Messages", 
"Mamey.Bank.ReserveBanks", "Mamey.Bank.Rewards", "Mamey.Bank.Savings", 
"Mamey.Bank.Shared", "Mamey.Bank.Transactions", "Mamey.Bank.Transfers", 
"Mamey.Bank.Treasury", "Mamey.Bank.Trusts", "Mamey.Bank.BlazorWasm", 
"Mamey.Bank.Withdraws", "Mamey.Businesses", "Mamey.Documents", "Mamey.Employees", 
"Mamey.Identity", "Mamey.KYC", "Mamey.Notifications", "Mamey.Organizations", 
"Mamey.People", "Mamey.UI.Core", "Mamey.UI.Maui", "Mamey.UI.Razor", "Mamey.Terminal", 
"Mamey.UI.Web.External", "Mamey.UI.Web.Internal", "Mamey.Wallets")

foreach($repo in $repos) {
    Write-Host "=========================================="
    Write-Host "Cloning the Repository: "$repo
    Write-Host "=========================================="
    $repo_url = "https://github.com/futureheadgroup/"+$repo
    git clone $repo_url
}
