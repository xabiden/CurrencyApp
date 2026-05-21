SELECT 'CREATE DATABASE currencyapp_users'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'currencyapp_users')\gexec

SELECT 'CREATE DATABASE currencyapp_finance'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'currencyapp_finance')\gexec
