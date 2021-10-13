-- ★テーブル定義取得用SQL
--この部分は任意のデータベース名に変更
use master
--全テーブルリスト
select * from
(
SELECT
    (select name from sys.schemas where T.schema_id = schema_id) スキーマ,
    T.name AS 表名,
    C.column_id AS 列番,
    C.name AS 列名,
    Y.name AS 型,
    CASE 
    WHEN Y.name IN ('nvarchar', 'nchar') THEN C.max_length / 2 
    WHEN C.precision = 0 THEN C.max_length 
    ELSE C.precision 
    END AS 桁,
    C.scale AS 小数桁,
    C.max_length as [サイズ(バイト)],
    C.is_nullable AS Null可,
    (select COUNT(*) from sys.index_columns ic where ic.column_id = C.column_id and C.object_id = ic.object_id and exists (select * from sys.key_constraints kc where kc.type = 'PK' and kc.parent_object_id = T.object_id and kc.unique_index_id = ic.index_id )) as [PK],
    ep.value as 説明
FROM
    sys.tables AS T
    INNER JOIN sys.columns AS C ON T.object_id = C.object_id
    INNER JOIN sys.types AS Y ON C.system_type_id = Y.system_type_id AND C.user_type_id = Y.user_type_id
    left OUTER JOIN sys.extended_properties AS ep ON C.object_id = ep.major_id and C.column_id = ep.minor_id
) z
order by z.スキーマ, z.表名, z.列番