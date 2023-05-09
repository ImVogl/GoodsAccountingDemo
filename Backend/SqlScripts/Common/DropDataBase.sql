ALTER DATABASE goods_account CONNECTION LIMIT 0;
SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = 'goods_account';
DROP DATABASE goods_account;