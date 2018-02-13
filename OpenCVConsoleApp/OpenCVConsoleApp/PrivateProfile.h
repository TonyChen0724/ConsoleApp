#ifndef	_PRIVATEPROFILE_HEADER_2005_01_27_
#define	_PRIVATEPROFILE_HEADER_2005_01_27_

//non-sort
#pragma warning(disable:4786) //disable linker key > 255 chars long (for stl map)
#include	<vector>
using namespace std;

//
class	PRIVATEPROFILEKEY;
class	PRIVATEPROFILE;
class	PRIVATEPROFILESECTION;

//
typedef	PRIVATEPROFILEKEY*		LPPRIVATEPROFILEKEY;
typedef	PRIVATEPROFILESECTION*	LPPRIVATEPROFILESECTION;
typedef	PRIVATEPROFILE*			LPPRIVATEPROFILE;

//
static char	szPrivateProfileBuffer[256] = { 0 };

//----------------------------------------------------------------------------
class	PRIVATEPROFILEKEY
{
private:
	char						*pKey;		//Key 名稱
	char						*pValues;	//Values

public:
	inline						PRIVATEPROFILEKEY();
	inline						PRIVATEPROFILEKEY( char *szKey, char *szValues );
	inline						PRIVATEPROFILEKEY( char *szKey, int iValuess );
	inline						~PRIVATEPROFILEKEY();
	//釋放
	inline	void				Release();

	//取得 Key 名稱
	//返回:
	//		Key字串位址
	inline	char*				GetKey();

	//設定 Key 名稱
	//參數:
	//		char *szKey			//Key name的字串, 不可為NULL或空字串
	//返回:
	//		失敗FALSE, 成功TRUE
	inline	BOOL				SetKey( char *szKey );

	//取得 Values(String Type)
	//返回:
	//		Values字串位址
	inline	char*				GetValuesString();

	//設定 Values(String Type)
	//參數:
	//		char *szValues		//Values的字串
	inline	void				SetValuesString( char *szValues=NULL );

	//取得 Values(Int Type)
	//返回:
	//		Values數值
	inline	int					GetValuesInt();

	inline float				GetValuesFloat();

	//設定 Values(Int Type)
	//參數:
	//		int iValues			//Values的值
	inline	void				SetValuesInt( int iValues=0 );
};
//----------------------------------------------------------------------------
#define	STL_VECTOR_KEY		vector< PRIVATEPROFILEKEY* >
class	PRIVATEPROFILESECTION
{
private:
	char						*pSection;		//Section 名稱
	STL_VECTOR_KEY				vKey;			//Key 數組

public:
	inline						PRIVATEPROFILESECTION();
	inline						~PRIVATEPROFILESECTION();
	//釋放
	inline	void				Release();

	//取得 Section 裡的 Key 數量
	//返回:
	//		Key的數量
	inline	int					Count();

	//取得 Section 名稱
	//返回:
	//		Section字串位址
	inline	char*				GetSection();

	//設定 Section 名稱
	//參數:
	//		char *szSection		//Section name的字串, 不可為NULL或空字串
	//返回:
	//		失敗FALSE, 成功TRUE
	inline	BOOL				SetSection( char *szSection );

	//刪除指定索引編號的 Key 項
	//參數:
	//		int index			//索引編號
	inline	void				DelKey( int index );

	//刪除指定 Key 名稱的 Key 項
	//參數:
	//		char *szKey			//Key name字串, 不可為NULL或空字串
	inline	void				DelKey( char *szKey );

	//取得指定索引編號的 Key 項
	//參數:
	//		int index			//索引編號
	//返回:
	//		失敗NULL, 成功取得PRIVATEPROFILEKEY內存位址
	inline	LPPRIVATEPROFILEKEY	GetKey( int index );

	//取得指定 Key 名稱的 Key 項
	//參數:
	//		char *szKey			//Key name字串, 不可為NULL或空字串
	//返回:
	//		失敗NULL, 成功取得PRIVATEPROFILEKEY內存位址
	inline	LPPRIVATEPROFILEKEY	GetKey( char *szKey );

	//設定 Key 項的 Value(String Type)
	//如果沒有指定的 Key 項, 則產生新 Key 項
	//參數:
	//		char *szKey			//Key name字串, 不可為NULL或空字串
	//		char *szValues		//Values的字串
	//返回:
	//		失敗NULL, 成功取得PRIVATEPROFILEKEY內存位址
	inline	LPPRIVATEPROFILEKEY	SetValuesString( char *szKey, char *szValues=NULL );

	//設定 Key 項的 Value(Int Type)
	//如果沒有指定的 Key 項, 則產生新 Key 項, 並設定 Key & Value
	//參數:
	//		char *szKey			//Key name字串, 不可為NULL或空字串
	//		int iValues			//Values的值
	//返回:
	//		失敗NULL, 成功取得PRIVATEPROFILEKEY內存位址
	inline	LPPRIVATEPROFILEKEY	SetValuesInt( char *szKey, int iValues=0 );
};
//----------------------------------------------------------------------------
#define	STL_VECTOR_SECTION	vector< PRIVATEPROFILESECTION* >
class PRIVATEPROFILE
{
private:
	STL_VECTOR_SECTION	vSection;	//Section 數組
	int					iMaxLineBuffer;
	int					iMaxSectionBuffer;
	int					iMaxKeyBuffer;
	int					iMaxValuesBuffer;
	char				*pLineBuffer;
	char				*pSectionBuffer;
	char				*pKeyBuffer;
	char				*pValuesBuffer;

public:
	inline							PRIVATEPROFILE();
	inline							~PRIVATEPROFILE();
	//釋放
	inline	void					Release();

	//取得 Section 的數量
	inline	int						Count();


	//Section 相關
	//取得指定索引編號的 Section 項
	//參數:
	//		int index				//索引編號
	//返回:
	//		失敗NULL, 成功PRIVATEPROFILESECTION內存位址
	inline	LPPRIVATEPROFILESECTION	GetSection( int index );

	//取得指定 Section 名稱的 Section 項
	//參數:
	//		char *szSection			//Section name字串, 不可為NULL或空字串
	//返回:
	//		失敗NULL, 成功PRIVATEPROFILESECTION內存位址
	inline	LPPRIVATEPROFILESECTION	GetSection( char *szSection );

	//設定 Section 名稱
	//如果該Section項存在, 返回該Section, 否則新增加一個 Section 項
	//參數:
	//		char *szSection			//Section name字串, 不可為NULL或空字串
	//返回:
	//		失敗NULL, 成功PRIVATEPROFILESECTION內存位址
	inline	LPPRIVATEPROFILESECTION	SetSection( char *szSection );

	//刪除指定索引編號的 Section 項
	//參數:
	//		int index				//索引編號
	inline	void					DelSection( int index );

	//刪除指定 Section 名稱的 Section 項
	//參數:
	//		char *szSection			//Section name字串, 不可為NULL或空字串
	inline	void					DelSection( char *szSection );


	//基本成員函數
	//取得指定Section名稱與Key名稱的Values(String Type)
	//參數:
	//		char *szSection			//Section name字串, 不可為NULL或空字串
	//		char *szKey				//Key name字串, 不可為NULL或空字串
	//返回:
	//		Values的字串
	inline	char*					GetPrivateProfileString( char *szSection, char *szKey );

	//設定指定Section名稱與Key名稱的Values(String Type)
	//參數:
	//		char *szSection			//Section name字串, 不可為NULL或空字串
	//		char *szKey				//Key name字串, 不可為NULL或空字串
	//		char *szValues			//Values的字串
	inline	void					SetPrivateProfileString( char *szSection, char *szKey, char *szValues );

	//取得指定Section名稱與Key名稱的Values(Int Type)
	//參數:
	//		char *szSection			//Section name字串, 不可為NULL或空字串
	//		char *szKey				//Key name字串, 不可為NULL或空字串
	//返回:
	//		Values的值
	inline	int						GetPrivateProfileInt( char *szSection, char *szKey );

	//設定指定Section名稱與Key名稱的Values(Int Type)
	//參數:
	//		char *szSection			//Section name字串, 不可為NULL或空字串
	//		char *szKey				//Key name字串, 不可為NULL或空字串
	//		int iValues				//Values的值
	inline	void					SetPrivateProfileInt( char *szSection, char *szKey, int iValues );


	//檔案相關
	//開啟指定檔案
	//參數:
	//		char *szFileName		//檔案路徑名稱
	//返回:
	//		失敗FALSE, 成功TRUE
	inline	BOOL					OpenFile( const char *szFileName );

	//寫入指定檔案
	//參數:
	//		char *szFileName		//檔案路徑名稱
	//返回:
	//		失敗FALSE, 成功TRUE
	inline	BOOL					WriteFile( char *szFileName );

	//開啟指定編碼檔案(編碼自訂)
	//參數:
	//		char	*szFileName		//檔案路徑名稱
	//		int		iCodeCount		//編碼數量
	//		char	*pDeCode		//編碼數組位址
	//返回:
	//		失敗FALSE, 成功TRUE
	inline	BOOL					OpenBinFile( char *szFileName, int iCodeCount, char *pDeCode );

	//寫入指定編碼檔案(編碼自訂)
	//參數:
	//		char	*szFileName		//檔案路徑名稱
	//		int		iCodeCount		//編碼數量
	//		char	*pEnCode		//編碼數組位址
	//返回:
	//		失敗FALSE, 成功TRUE
	inline	BOOL					WriteBinFile( char *szFileName, int iCodeCount, char *pEnCode );
};
//============================================================================
inline
PRIVATEPROFILEKEY::PRIVATEPROFILEKEY()
{
	pKey = NULL;
	pValues = NULL;
}
inline
PRIVATEPROFILEKEY::PRIVATEPROFILEKEY( char *szKey, char *szValues )
{
	pKey = NULL;
	pValues = NULL;
	SetKey( szKey );
	SetValuesString( szValues );
}
inline
PRIVATEPROFILEKEY::PRIVATEPROFILEKEY( char *szKey, int iValuess )
{
	pKey = NULL;
	pValues = NULL;
	SetKey( szKey );
	SetValuesInt( iValuess );
}
inline
PRIVATEPROFILEKEY::~PRIVATEPROFILEKEY()
{
	Release();
}
inline
void
PRIVATEPROFILEKEY::Release()
{
	if( pKey )
	{
		delete [] pKey;
		pKey = NULL;
	}//if
	if( pValues )
	{
		delete [] pValues;
		pValues = NULL;
	}//if
}
//取得 Key 名稱
inline
char*
PRIVATEPROFILEKEY::GetKey()
{
	return pKey;
}
//設定 Key 名稱
inline
BOOL
PRIVATEPROFILEKEY::SetKey( char *szKey )
{
	if( ( NULL == szKey ) || ( 0 == strlen( szKey ) ) )
		return FALSE;

	if( pKey )
	{
		delete [] pKey;
	}//if
	pKey = new char[ strlen( szKey ) + 1 ];
	strcpy_s( pKey, strlen(szKey) + 1, szKey );

	return TRUE;
}
//取得 Values(String Type)
inline
char*
PRIVATEPROFILEKEY::GetValuesString()
{
	return pValues;
}
//設定 Values(String Type)
inline
void
PRIVATEPROFILEKEY::SetValuesString( char *szValues )
{
	if( pValues )
	{
		delete [] pValues;
		pValues = NULL;
	}//if

	if( ( szValues ) && ( 0 < strlen( szValues ) ) )
	{
		pValues = new char[ strlen( szValues ) + 1 ];
		strcpy_s( pValues, strlen(szValues) + 1, szValues );
	}//if
}
inline
float
PRIVATEPROFILEKEY::GetValuesFloat()
{
	if (pValues)
		return atof(pValues);

	return 0.f;
}

//取得 Values(Int Type)
inline
int
PRIVATEPROFILEKEY::GetValuesInt()
{
	if( pValues )
		return atoi( pValues );

	return 0;
}
//設定 Values(Int Type)
inline
void
PRIVATEPROFILEKEY::SetValuesInt( int iValues )
{
	sprintf_s( szPrivateProfileBuffer, "%d", iValues );
	SetValuesString( szPrivateProfileBuffer );
}
//============================================================================
inline
PRIVATEPROFILESECTION::PRIVATEPROFILESECTION()
{
	pSection = NULL;
}
inline
PRIVATEPROFILESECTION::~PRIVATEPROFILESECTION()
{
	Release();
}
inline
void
PRIVATEPROFILESECTION::Release()
{
	if( pSection )
	{
		delete [] pSection;
		pSection = NULL;
	}//if
	for( int i = 0; i < vKey.size(); i++ )
	{
		delete vKey[ i ];
		vKey[ i ] = NULL;
	}//for
	vKey.clear();
}
//取得 Key 的數量
inline
int
PRIVATEPROFILESECTION::Count()
{
	return vKey.size();
}
//取得 Section 名稱
inline
char*
PRIVATEPROFILESECTION::GetSection()
{
	return pSection;
}
//設定 Section 名稱
inline
BOOL
PRIVATEPROFILESECTION::SetSection( char *szSection )
{
	if( ( NULL == szSection ) || ( 0 == strlen( szSection ) ) )
		return FALSE;

	if( pSection )
	{
		delete [] pSection;
	}//if
	pSection = new char[ strlen( szSection ) + 1 ];
	strcpy_s( pSection, strlen(szSection) + 1, szSection );

	return TRUE;
}
//刪除指定索引編號的 Key 項
inline
void
PRIVATEPROFILESECTION::DelKey( int index )
{
	if( ( 0 > index ) || ( vKey.size() <= index ) )
		return;

	delete vKey[ index ];
	vKey[ index ] = NULL;
	vKey.erase( vKey.begin() + index );
}
//刪除指定 Key 名稱的 Key 項
inline
void
PRIVATEPROFILESECTION::DelKey( char *szKey )
{
	if( ( NULL == szKey ) || ( 0 == strlen( szKey ) ) )
		return;

	for( int i = 0; i < vKey.size(); i++ )
	{
		if( 0 == _strcmpi( vKey[ i ]->GetKey(), szKey ) )
		{
			delete vKey[ i ];
			vKey[ i ] = NULL;
			vKey.erase( vKey.begin() + i );
			return;
		}//if
	}//for
}
//取得指定索引編號的 Key 項
inline
LPPRIVATEPROFILEKEY
PRIVATEPROFILESECTION::GetKey( int index )
{
	if( ( 0 > index ) || ( vKey.size() <= index ) )
	{
		return NULL;
	}//if
	return vKey[ index ];
}
//取得指定 Key 名稱的 Key 項
inline
LPPRIVATEPROFILEKEY
PRIVATEPROFILESECTION::GetKey( char *szKey )
{
	if( ( NULL == szKey ) || ( 0 == strlen( szKey ) ) )
		return NULL;

	for( int i = 0; i < vKey.size(); i++ )
	{
		if( 0 == _strcmpi( vKey[ i ]->GetKey(), szKey ) )
			return vKey[ i ];
	}//for

	return NULL;
}
//設定 Key 項的 Value(String Type), 如果沒有指定的 Key 項, 則產生新 Key 項, 並設定 Key & Value
inline
LPPRIVATEPROFILEKEY
PRIVATEPROFILESECTION::SetValuesString( char *szKey, char *szValues )
{
	if( ( NULL == szKey ) || ( 0 == strlen( szKey ) ) )
		return NULL;

	for( int i = 0; i < vKey.size(); i++ )
	{
		if( 0 == _strcmpi( vKey[ i ]->GetKey(), szKey ) )
		{
			vKey[ i ]->SetValuesString( szValues );
			return vKey[ i ];
		}//if
	}//for

	LPPRIVATEPROFILEKEY	pfKey;
	pfKey = new PRIVATEPROFILEKEY( szKey, szValues );
	vKey.push_back( pfKey );

	return pfKey;
}
//設定 Key 項的 Value(Int Type), 如果沒有指定的 Key 項, 則產生新 Key 項, 並設定 Key & Value
inline
LPPRIVATEPROFILEKEY
PRIVATEPROFILESECTION::SetValuesInt( char *szKey, int iValues )
{
	if( ( NULL == szKey ) || ( 0 == strlen( szKey ) ) )
		return NULL;


	for( int i = 0; i < vKey.size(); i++ )
	{
		if( 0 == _strcmpi( vKey[ i ]->GetKey(), szKey ) )
		{
			vKey[ i ]->SetValuesInt( iValues );
			return vKey[ i ];
		}//if
	}//for

	LPPRIVATEPROFILEKEY	pfKey;
	pfKey = new PRIVATEPROFILEKEY( szKey, iValues );
	vKey.push_back( pfKey );

	return pfKey;
}
//============================================================================
inline
PRIVATEPROFILE::PRIVATEPROFILE()
{
	iMaxLineBuffer		= 256;
	iMaxSectionBuffer	= 256;
	iMaxKeyBuffer		= 256;
	iMaxValuesBuffer	= 256;
	pLineBuffer		= NULL;
	pSectionBuffer	= NULL;
	pKeyBuffer		= NULL;
	pValuesBuffer	= NULL;
}
inline
PRIVATEPROFILE::~PRIVATEPROFILE()
{
	Release();

	if( pLineBuffer )
		delete [] pLineBuffer;
	if( pSectionBuffer )
		delete [] pSectionBuffer;
	if( pKeyBuffer )
		delete [] pKeyBuffer;
	if( pValuesBuffer )
		delete [] pValuesBuffer;
}
inline
void
PRIVATEPROFILE::Release()
{
	for( int i = 0; i < vSection.size(); i++ )
	{
		delete vSection[ i ];
		vSection[ i ] = NULL;
	}//for
	vSection.clear();
}
//取得 Section 的數量
inline
int
PRIVATEPROFILE::Count()
{
	return vSection.size();
}
//取得指定索引編號的 Section 項
inline
LPPRIVATEPROFILESECTION
PRIVATEPROFILE::GetSection( int index )
{
	if( ( 0 > index ) || ( vSection.size() <= index ) )
	{
		return NULL;
	}//if
	return vSection[ index ];
}
//取得指定 Section 名稱的 Section 項
inline
LPPRIVATEPROFILESECTION
PRIVATEPROFILE::GetSection( char *szSection )
{
	if( ( NULL == szSection ) || ( 0 == strlen( szSection ) ) )
		return NULL;

	for( int i = 0; i < vSection.size(); i++ )
	{
		if( 0 == _strcmpi( vSection[ i ]->GetSection(), szSection ) )
			return vSection[ i ];
	}//for

	return NULL;
}
//設定 Section 名稱
inline
LPPRIVATEPROFILESECTION
PRIVATEPROFILE::SetSection( char *szSection )
{
	if( ( NULL == szSection ) || ( 0 == strlen( szSection ) ) )
		return NULL;

	for( int i = 0; i < vSection.size(); i++ )
	{
		if( 0 == _strcmpi( vSection[ i ]->GetSection(), szSection ) )
			return vSection[ i ];
	}//for

	LPPRIVATEPROFILESECTION	pfSection;
	pfSection = new PRIVATEPROFILESECTION;
	pfSection->SetSection( szSection );
	vSection.push_back( pfSection );

	return pfSection;
}
//刪除指定索引編號的 Section 項
inline
void
PRIVATEPROFILE::DelSection( int index )
{
	if( ( 0 > index ) || ( vSection.size() <= index ) )
		return;

	delete vSection[ index ];
	vSection[ index ] = NULL;
	vSection.erase( vSection.begin() + index );
}
//刪除指定 Section 名稱的 Section 項
inline
void
PRIVATEPROFILE::DelSection( char *szSection )
{
	if( ( NULL == szSection ) || ( 0 == strlen( szSection ) ) )
		return;

	for( int i = 0; i < vSection.size(); i++ )
	{
		if( 0 == _strcmpi( vSection[ i ]->GetSection(), szSection ) )
		{
			delete vSection[ i ];
			vSection[ i ] = NULL;
			vSection.erase( vSection.begin() + i );
			return;
		}//if
	}//for
}
//取得指定Section名稱與Key名稱的Values(String Type)
inline
char*
PRIVATEPROFILE::GetPrivateProfileString( char *szSection, char *szKey )
{
	if( ( NULL == szSection ) || ( 0 == strlen( szSection ) ) )
		return NULL;
	if( ( NULL == szKey ) || ( 0 == strlen( szKey ) ) )
		return NULL;

	LPPRIVATEPROFILESECTION	pfSection;
	pfSection = GetSection( szSection );
	if( NULL == pfSection )
		return NULL;

	LPPRIVATEPROFILEKEY	pfKey;
	pfKey = pfSection->GetKey( szKey );
	if( NULL == pfKey )
		return NULL;
	return pfKey->GetValuesString();
}
//設定指定Section名稱與Key名稱的Values(String Type)
inline
void
PRIVATEPROFILE::SetPrivateProfileString( char *szSection, char *szKey, char *szValues )
{
	if( ( NULL == szSection ) || ( 0 == strlen( szSection ) ) )
		return;
	if( ( NULL == szKey ) || ( 0 == strlen( szKey ) ) )
		return;

	LPPRIVATEPROFILESECTION	pfSection;
	pfSection = SetSection( szSection );
	if( NULL == pfSection )
		return;

	pfSection->SetValuesString( szKey, szValues );
}
//取得指定Section名稱與Key名稱的Values(Int Type)
inline
int
PRIVATEPROFILE::GetPrivateProfileInt( char *szSection, char *szKey )
{
	if( ( NULL == szSection ) || ( 0 == strlen( szSection ) ) )
		return NULL;
	if( ( NULL == szKey ) || ( 0 == strlen( szKey ) ) )
		return NULL;

	LPPRIVATEPROFILESECTION	pfSection;
	pfSection = GetSection( szSection );
	if( NULL == pfSection )
		return NULL;

	LPPRIVATEPROFILEKEY	pfKey;
	pfKey = pfSection->GetKey( szKey );
	if( NULL == pfKey )
		return NULL;

	return pfKey->GetValuesInt();
}
//設定指定Section名稱與Key名稱的Values(Int Type)
inline
void
PRIVATEPROFILE::SetPrivateProfileInt( char *szSection, char *szKey, int iValues )
{
	if( ( NULL == szSection ) || ( 0 == strlen( szSection ) ) )
		return;
	if( ( NULL == szKey ) || ( 0 == strlen( szKey ) ) )
		return;

	LPPRIVATEPROFILESECTION	pfSection;
	pfSection = SetSection( szSection );
	if( NULL == pfSection )
		return;

	pfSection->SetValuesInt( szKey, iValues );
}
//開啟指定檔案
inline
BOOL
PRIVATEPROFILE::OpenFile( const char *szFileName )
{
	FILE	*hFile;
	int		iLineLength, iSectionLength, iKeyLength, iValuesLength;
	char	*pStrFind;		//尋找字元
	char	*pTempBuffer;	//暫存用
	char	cStr;

	errno_t err;
	err = fopen_s(&hFile, szFileName, "rt");
	if (err != 0)
	{
		return FALSE;
	}
	/*if( NULL == ( hFile = fopen_s( szFileName, "rt") ) )
		return FALSE;*/

	if( NULL == pLineBuffer )
		pLineBuffer		= new char[ iMaxLineBuffer ];
	if( NULL == pSectionBuffer )
		pSectionBuffer	= new char[ iMaxSectionBuffer ];
	if( NULL == pKeyBuffer )
		pKeyBuffer		= new char[ iMaxKeyBuffer ];
	if( NULL == pValuesBuffer )
		pValuesBuffer	= new char[ iMaxValuesBuffer ];

	iLineLength = 0;
	while( 1 )
	{
		fread( &cStr, 1, 1, hFile );
		if( ( cStr == '\n' ) || ( feof( hFile ) ) )
		{
			pLineBuffer[ iLineLength ] = 0;
			if( ';' == pLineBuffer[0] )
			{
				iLineLength = 0;
				continue;
			} else if( ( '[' == pLineBuffer[0] ) && ( ']' == pLineBuffer[ iLineLength - 1] ) )
			{	//Section
				iSectionLength = strlen( pLineBuffer ) - 2;
				if( iMaxSectionBuffer <= iSectionLength )
				{	//增大Section Buffer
					delete [] pSectionBuffer;
					iMaxSectionBuffer += ( iSectionLength - iMaxSectionBuffer + 1 );
					pSectionBuffer = new char[ iMaxSectionBuffer ];
				}//if
				memset( pSectionBuffer, 0, iMaxSectionBuffer );
				memcpy( pSectionBuffer, pLineBuffer + 1, iSectionLength );
				SetSection( pSectionBuffer );
			} else if( 0 < strlen( pLineBuffer ) )
			{	//Key & Values
				if( NULL != ( pStrFind = strstr( pLineBuffer, "=" ) ) )
				{
					iKeyLength = pStrFind - pLineBuffer;
					if( iMaxKeyBuffer <= iKeyLength )
					{	//增大Key Buffer
						delete [] pKeyBuffer;
						iMaxKeyBuffer += ( iKeyLength - iMaxKeyBuffer + 1 );
						pKeyBuffer = new char[ iMaxKeyBuffer ];
					}//if
					memcpy( pKeyBuffer, pLineBuffer, iKeyLength );
					pKeyBuffer[ iKeyLength ] = 0;
					if( 0 < strlen( pKeyBuffer ) )
					{
						iValuesLength = strlen( pLineBuffer ) - ( pStrFind - pLineBuffer + 1 );
						if( iMaxValuesBuffer <= iValuesLength )
						{	//增大Values Buffer
							delete [] pValuesBuffer;
							iMaxValuesBuffer += ( iValuesLength - iMaxValuesBuffer + 1 );
							pValuesBuffer = new char[ iMaxValuesBuffer ];
						}//if
						memcpy( pValuesBuffer, pStrFind + 1, iValuesLength );
						pValuesBuffer[ iValuesLength ] = 0;
						SetPrivateProfileString( pSectionBuffer, pKeyBuffer, pValuesBuffer );
					}//if
				}//if
			} else if( feof( hFile ) )
			{
				break;
			}//if-else
			iLineLength = 0;
			continue;
		} else if( 0x09 == cStr )	//Tab
		{
			continue;
		}//if-else
		while( iMaxLineBuffer <= iLineLength )
		{	//增大Line Buffer
			pTempBuffer = new char[ iMaxLineBuffer + 256 ];
			memcpy( pTempBuffer, pLineBuffer, iMaxLineBuffer );
			delete [] pLineBuffer;
			pLineBuffer = pTempBuffer;
			iMaxLineBuffer += 256;
		}//while
		pLineBuffer[ iLineLength++ ] = cStr;
	}//while

	fclose( hFile );

	return TRUE;
}
//寫入指定檔案
inline
BOOL
PRIVATEPROFILE::WriteFile( char *szFileName )
{
	FILE	*hFile;
	LPPRIVATEPROFILESECTION	pfSection;
	LPPRIVATEPROFILEKEY		pfKey;
	errno_t err;
	err = fopen_s(&hFile, szFileName, "w+");
	if (err != 0)
	{
		return FALSE;
	}
	/*if( NULL == ( hFile = fopen_s( szFileName, "w+") ) )
		return FALSE;*/

	for( int iSection = 0; iSection < vSection.size(); iSection++ )
	{
		pfSection = vSection[ iSection ];
		fprintf( hFile, "[%s]\n", pfSection->GetSection() );
		for( int iKey = 0; iKey < pfSection->Count(); iKey++ )
		{
			pfKey = pfSection->GetKey( iKey );
			fprintf( hFile, "%s=%s\n", pfKey->GetKey(), pfKey->GetValuesString() );
		}//for
		fprintf( hFile, "\n" );
	}//for

	fclose( hFile );
	return TRUE;
}
//開啟指定編碼檔案(編碼自訂)
inline
BOOL
PRIVATEPROFILE::OpenBinFile( char *szFileName, int iCodeCount, char *pDeCode )
{
	FILE	*hFile;
	int		iLineLength, iDeCodeLoop;
	int		iSectionLength, iKeyLength, iValuesLength;
	char	*pStrFind;		//尋找字元
	char	*pTempBuffer;	//暫存用
	char	cStr;

	if( ( 0 >= iCodeCount ) || ( NULL == pDeCode ) )
		return FALSE;
	errno_t err;
	err = fopen_s(&hFile,szFileName, "rb");
	if (err != 0)
	{
		return FALSE;
	}
	/*if( NULL == ( hFile = fopen_s( szFileName, "rb") ) )
		return FALSE;*/

	if( NULL == pLineBuffer )
		pLineBuffer		= new char[ iMaxLineBuffer ];
	if( NULL == pSectionBuffer )
		pSectionBuffer	= new char[ iMaxSectionBuffer ];
	if( NULL == pKeyBuffer )
		pKeyBuffer		= new char[ iMaxKeyBuffer ];
	if( NULL == pValuesBuffer )
		pValuesBuffer	= new char[ iMaxValuesBuffer ];

	iDeCodeLoop = 0;
	iLineLength = 0;
	while( 1 )
	{
		fread( &cStr, 1, 1, hFile );
		cStr -= pDeCode[iDeCodeLoop];
		iDeCodeLoop = ( iDeCodeLoop + 1 ) % iCodeCount;
		if( ( cStr == '\n' ) || ( feof( hFile ) ) )
		{
			pLineBuffer[ iLineLength ] = 0;
			if( ';' == pLineBuffer[0] )
			{
				iLineLength = 0;
				continue;
			} else if( ( '[' == pLineBuffer[0] ) && ( ']' == pLineBuffer[ iLineLength - 1] ) )
			{	//Section
				iSectionLength = strlen( pLineBuffer ) - 2;
				if( iMaxSectionBuffer <= iSectionLength )
				{	//增大Section Buffer
					delete [] pSectionBuffer;
					iMaxSectionBuffer += ( iSectionLength - iMaxSectionBuffer + 1 );
					pSectionBuffer = new char[ iMaxSectionBuffer ];
				}//if
				memset( pSectionBuffer, 0, iMaxSectionBuffer );
				memcpy( pSectionBuffer, pLineBuffer + 1, iSectionLength );
				SetSection( pSectionBuffer );
			} else if( 0 < strlen( pLineBuffer ) )
			{	//Key & Values
				if( NULL != ( pStrFind = strstr( pLineBuffer, "=" ) ) )
				{
					iKeyLength = pStrFind - pLineBuffer;
					if( iMaxKeyBuffer <= iKeyLength )
					{	//增大Key Buffer
						delete [] pKeyBuffer;
						iMaxKeyBuffer += ( iKeyLength - iMaxKeyBuffer + 1 );
						pKeyBuffer = new char[ iMaxKeyBuffer ];
					}//if
					memcpy( pKeyBuffer, pLineBuffer, iKeyLength );
					pKeyBuffer[ iKeyLength ] = 0;
					if( 0 < strlen( pKeyBuffer ) )
					{
						iValuesLength = strlen( pLineBuffer ) - ( pStrFind - pLineBuffer + 1 );
						if( iMaxValuesBuffer <= iValuesLength )
						{	//增大Values Buffer
							delete [] pValuesBuffer;
							iMaxValuesBuffer += ( iValuesLength - iMaxValuesBuffer + 1 );
							pValuesBuffer = new char[ iMaxValuesBuffer ];
						}//if
						memcpy( pValuesBuffer, pStrFind + 1, iValuesLength );
						pValuesBuffer[ iValuesLength ] = 0;
						SetPrivateProfileString( pSectionBuffer, pKeyBuffer, pValuesBuffer );
					}//if
				}//if
			} else if( feof( hFile ) )
			{
				break;
			}//if-else
			iLineLength = 0;
			continue;
		} else if( 0x09 == cStr )	//Tab
		{
			continue;
		}//if-else
		while( iMaxLineBuffer <= iLineLength )
		{	//增大Line Buffer
			pTempBuffer = new char[ iMaxLineBuffer + 256 ];
			memcpy( pTempBuffer, pLineBuffer, iMaxLineBuffer );
			delete [] pLineBuffer;
			pLineBuffer = pTempBuffer;
			iMaxLineBuffer += 256;
		}//while
		pLineBuffer[ iLineLength++ ] = cStr;
	}//while

	fclose( hFile );

	return TRUE;
}
//寫入指定編碼檔案(編碼自訂)
inline
BOOL
PRIVATEPROFILE::WriteBinFile( char *szFileName, int iCodeCount, char *pEnCode )
{
	FILE					*hFile;
	LPPRIVATEPROFILESECTION	pfSection;
	LPPRIVATEPROFILEKEY		pfKey;
	int						i, iSection, iKey;
	int						iSectionLength, iLineLength;
	int						iDeCodeLoop;

	if( ( 0 >= iCodeCount ) || ( NULL == pEnCode ) )
		return FALSE;
	errno_t err;
	err = fopen_s(&hFile, szFileName, "wb+");
	if (err != 0)
	{
		return FALSE;
	}
	//if( NULL == ( hFile = fopen_s( szFileName, "wb+") ) )
	//	return FALSE;

	if( NULL == pLineBuffer )
		pLineBuffer		= new char[ iMaxLineBuffer ];
	if( NULL == pSectionBuffer )
		pSectionBuffer	= new char[ iMaxSectionBuffer ];

	iDeCodeLoop = 0;
	for( iSection = 0; iSection < vSection.size(); iSection++)
	{
		pfSection = vSection[ iSection ];
		//Section
		iSectionLength = strlen( pfSection->GetSection() ) + 3;
		if( iMaxSectionBuffer <= iSectionLength )
		{	//增大Section Buffer
			delete [] pSectionBuffer;
			iMaxSectionBuffer += ( iSectionLength - iMaxSectionBuffer + 1 );
			pSectionBuffer = new char[ iMaxSectionBuffer ];
		}//if
		sprintf_s( pSectionBuffer, iSectionLength, "[%s]\n", pfSection->GetSection() );
		//Section字串編碼
		for( i = 0; i < iSectionLength; i++ )
		{
			pSectionBuffer[i] = ( pSectionBuffer[ i ]  + pEnCode[ iDeCodeLoop ] );
			iDeCodeLoop = ( iDeCodeLoop + 1 ) % iCodeCount;
		}//for
		//寫入檔案
		fwrite( pSectionBuffer, iSectionLength, 1, hFile );

		//Key
		for( iKey = 0; iKey < pfSection->Count(); iKey++ )
		{
			pfKey = pfSection->GetKey( iKey );
			iLineLength = strlen( pfKey->GetKey() ) + strlen( pfKey->GetValuesString() ) + 2;
			while( iMaxLineBuffer <= iLineLength )
			{	//增大Line Buffer
				delete [] pLineBuffer;
				iMaxLineBuffer += 256;
				pLineBuffer = new char[ iMaxLineBuffer + 256 ];
			}//while
			sprintf_s( pLineBuffer, iSectionLength, "%s=%s\n", pfKey->GetKey(), pfKey->GetValuesString() );
			//Key&Values字串編碼
			for( i = 0; i < iLineLength; i++ )
			{
				pLineBuffer[i] = ( pLineBuffer[ i ]  + pEnCode[ iDeCodeLoop ] );
				iDeCodeLoop = ( iDeCodeLoop + 1 ) % iCodeCount;
			}//for
			//寫入檔案
			fwrite( pLineBuffer, iLineLength, 1, hFile );
		}//for
	}//for

	fclose( hFile );

	return TRUE;
}

#endif