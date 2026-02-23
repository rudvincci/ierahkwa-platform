#!/bin/bash
REPOSITORIES=(Mamey Mamey Mamey.Accounts Mamey.Bank.Accounts 
Mamey.Bank.ApiGateway Mamey.Bank.Beneficiaries Mamey.Bank.Branches 
Mamey.Bank.Cards Mamey.Bank.CentralBanks Mamey.Bank.Correspondents 
Mamey.Bank.Deposits Mamey.Bank.Documents Mamey.Bank.HelpDesk Mamey.Bank.Ktt 
Mamey.Bank.Ktt.Parser Mamey.Bank.Ledgers  Mamey.Bank.Licenses 
Mamey.Bank.Loans Mamey.Bank.Members Mamey.Bank.Messages 
Mamey.Bank.ReserveBanks Mamey.Bank.Rewards Mamey.Bank.Savings 
Mamey.Bank.Shared Mamey.Bank.Transactions Mamey.Bank.Transfers 
Mamey.Bank.Treasury Mamey.Bank.Trusts Mamey.Bank.BlazorWasm 
Mamey.Bank.Withdraws Mamey.Businesses Mamey.Documents Mamey.Employees 
Mamey.Identity Mamey.KYC Mamey.Notifications Mamey.Organizations 
Mamey.People Mamey.UI.Core Mamey.UI.Maui Mamey.UI.Razor Mamey.Terminal 
Mamey.UI.Web.External Mamey.UI.Web.Internal Mamey.Wallets)

echo ${REPOSITORIES[@]} | sed -E -e 's/[[:blank:]]+/\n/g' | xargs -I {} -n 1 -P 0 sh -c 'printf "========================================================\nUpdating the repository: {}\n========================================================\n"; git -C {} checkout develop; git -C {} pull; git -C {} checkout master; git -C {} pull;git -C {} checkout develop;'
