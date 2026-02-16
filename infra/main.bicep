@description('Prefijo de nombres (ej: xipelabs-dev)')
param namePrefix string
param location string = resourceGroup().location

var storageName = toLower(replace('${namePrefix}stg', '-', ''))
var functionName = toLower('${namePrefix}-api')
var planName = '${namePrefix}-func-plan'

// Storage
resource stg 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageName
  location: location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
  }
}

// Container para evidencias
resource claimsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: '${stg.name}/default/claims'
}

// Connection string para Functions (Dev/MVP)
var stgKey = listKeys(stg.id, stg.apiVersion).keys[0].value
var storageConn = 'DefaultEndpointsProtocol=https;AccountName=${stg.name};AccountKey=${stgKey};EndpointSuffix=core.windows.net'

// Plan consumo
resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: planName
  location: location
  sku: { name: 'Y1', tier: 'Dynamic' }
}

// Function App (Linux + .NET 8 isolated)
resource func 'Microsoft.Web/sites@2023-12-01' = {
  name: functionName
  location: location
  kind: 'functionapp,linux'
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|8.0'
      appSettings: [
        { name: 'AzureWebJobsStorage', value: storageConn }
        { name: 'FUNCTIONS_WORKER_RUNTIME', value: 'dotnet-isolated' }
        { name: 'WEBSITE_RUN_FROM_PACKAGE', value: '1' }
        { name: 'STORAGE_ACCOUNT', value: stg.name }
      ]
    }
  }
}

output storageAccountName string = stg.name
output functionAppName string = func.name
