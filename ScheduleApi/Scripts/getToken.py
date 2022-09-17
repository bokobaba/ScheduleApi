import http.client

conn = http.client.HTTPSConnection("dev-0yfu4fcd.us.auth0.com")

payload = "{\"client_id\":\"a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH\",\
"client_secret\":\"BV4EIi5rqWkUcb8dFwO720dRMH2VFDb5ttL4cP18yvJIJ24zUFqwDE0ZfoGo_Id2\",\"audience\":\"https://scheduleapi20220831111508.azurewebsites.net/api\",\"grant_type\":\"client_credentials\"}"

headers = { 'content-type': "application/json" }

conn.request("POST", "/oauth/token", payload, headers)

res = conn.getresponse()
data = res.read()

print(data.decode("utf-8"))