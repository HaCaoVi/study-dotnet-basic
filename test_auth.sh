curl -i http://localhost:5033/api/auth/account
echo ""
curl -i http://localhost:5033/api/auth/login -d '{}' -H "Content-Type: application/json"
echo ""
