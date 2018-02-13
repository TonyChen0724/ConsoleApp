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
	char						*pKey;		//Key ���Q
	char						*pValues;	//Values

public:
	inline						PRIVATEPROFILEKEY();
	inline						PRIVATEPROFILEKEY( char *szKey, char *szValues );
	inline						PRIVATEPROFILEKEY( char *szKey, int iValuess );
	inline						~PRIVATEPROFILEKEY();
	//ጷ�
	inline	void				Release();

	//ȡ�� Key ���Q
	//����:
	//		Key�ִ�λַ
	inline	char*				GetKey();

	//�O�� Key ���Q
	//����:
	//		char *szKey			//Key name���ִ�, ���ɞ�NULL����ִ�
	//����:
	//		ʧ��FALSE, �ɹ�TRUE
	inline	BOOL				SetKey( char *szKey );

	//ȡ�� Values(String Type)
	//����:
	//		Values�ִ�λַ
	inline	char*				GetValuesString();

	//�O�� Values(String Type)
	//����:
	//		char *szValues		//Values���ִ�
	inline	void				SetValuesString( char *szValues=NULL );

	//ȡ�� Values(Int Type)
	//����:
	//		Values��ֵ
	inline	int					GetValuesInt();

	inline float				GetValuesFloat();

	//�O�� Values(Int Type)
	//����:
	//		int iValues			//Values��ֵ
	inline	void				SetValuesInt( int iValues=0 );
};
//----------------------------------------------------------------------------
#define	STL_VECTOR_KEY		vector< PRIVATEPROFILEKEY* >
class	PRIVATEPROFILESECTION
{
private:
	char						*pSection;		//Section ���Q
	STL_VECTOR_KEY				vKey;			//Key ���M

public:
	inline						PRIVATEPROFILESECTION();
	inline						~PRIVATEPROFILESECTION();
	//ጷ�
	inline	void				Release();

	//ȡ�� Section �e�� Key ����
	//����:
	//		Key�Ĕ���
	inline	int					Count();

	//ȡ�� Section ���Q
	//����:
	//		Section�ִ�λַ
	inline	char*				GetSection();

	//�O�� Section ���Q
	//����:
	//		char *szSection		//Section name���ִ�, ���ɞ�NULL����ִ�
	//����:
	//		ʧ��FALSE, �ɹ�TRUE
	inline	BOOL				SetSection( char *szSection );

	//�h��ָ��������̖�� Key �
	//����:
	//		int index			//������̖
	inline	void				DelKey( int index );

	//�h��ָ�� Key ���Q�� Key �
	//����:
	//		char *szKey			//Key name�ִ�, ���ɞ�NULL����ִ�
	inline	void				DelKey( char *szKey );

	//ȡ��ָ��������̖�� Key �
	//����:
	//		int index			//������̖
	//����:
	//		ʧ��NULL, �ɹ�ȡ��PRIVATEPROFILEKEY�ȴ�λַ
	inline	LPPRIVATEPROFILEKEY	GetKey( int index );

	//ȡ��ָ�� Key ���Q�� Key �
	//����:
	//		char *szKey			//Key name�ִ�, ���ɞ�NULL����ִ�
	//����:
	//		ʧ��NULL, �ɹ�ȡ��PRIVATEPROFILEKEY�ȴ�λַ
	inline	LPPRIVATEPROFILEKEY	GetKey( char *szKey );

	//�O�� Key 헵� Value(String Type)
	//����]��ָ���� Key �, �t�a���� Key �
	//����:
	//		char *szKey			//Key name�ִ�, ���ɞ�NULL����ִ�
	//		char *szValues		//Values���ִ�
	//����:
	//		ʧ��NULL, �ɹ�ȡ��PRIVATEPROFILEKEY�ȴ�λַ
	inline	LPPRIVATEPROFILEKEY	SetValuesString( char *szKey, char *szValues=NULL );

	//�O�� Key 헵� Value(Int Type)
	//����]��ָ���� Key �, �t�a���� Key �, �K�O�� Key & Value
	//����:
	//		char *szKey			//Key name�ִ�, ���ɞ�NULL����ִ�
	//		int iValues			//Values��ֵ
	//����:
	//		ʧ��NULL, �ɹ�ȡ��PRIVATEPROFILEKEY�ȴ�λַ
	inline	LPPRIVATEPROFILEKEY	SetValuesInt( char *szKey, int iValues=0 );
};
//----------------------------------------------------------------------------
#define	STL_VECTOR_SECTION	vector< PRIVATEPROFILESECTION* >
class PRIVATEPROFILE
{
private:
	STL_VECTOR_SECTION	vSection;	//Section ���M
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
	//ጷ�
	inline	void					Release();

	//ȡ�� Section �Ĕ���
	inline	int						Count();


	//Section ���P
	//ȡ��ָ��������̖�� Section �
	//����:
	//		int index				//������̖
	//����:
	//		ʧ��NULL, �ɹ�PRIVATEPROFILESECTION�ȴ�λַ
	inline	LPPRIVATEPROFILESECTION	GetSection( int index );

	//ȡ��ָ�� Section ���Q�� Section �
	//����:
	//		char *szSection			//Section name�ִ�, ���ɞ�NULL����ִ�
	//����:
	//		ʧ��NULL, �ɹ�PRIVATEPROFILESECTION�ȴ�λַ
	inline	LPPRIVATEPROFILESECTION	GetSection( char *szSection );

	//�O�� Section ���Q
	//���ԓSection헴���, ����ԓSection, ��t������һ�� Section �
	//����:
	//		char *szSection			//Section name�ִ�, ���ɞ�NULL����ִ�
	//����:
	//		ʧ��NULL, �ɹ�PRIVATEPROFILESECTION�ȴ�λַ
	inline	LPPRIVATEPROFILESECTION	SetSection( char *szSection );

	//�h��ָ��������̖�� Section �
	//����:
	//		int index				//������̖
	inline	void					DelSection( int index );

	//�h��ָ�� Section ���Q�� Section �
	//����:
	//		char *szSection			//Section name�ִ�, ���ɞ�NULL����ִ�
	inline	void					DelSection( char *szSection );


	//�����ɆT����
	//ȡ��ָ��Section���Q�cKey���Q��Values(String Type)
	//����:
	//		char *szSection			//Section name�ִ�, ���ɞ�NULL����ִ�
	//		char *szKey				//Key name�ִ�, ���ɞ�NULL����ִ�
	//����:
	//		Values���ִ�
	inline	char*					GetPrivateProfileString( char *szSection, char *szKey );

	//�O��ָ��Section���Q�cKey���Q��Values(String Type)
	//����:
	//		char *szSection			//Section name�ִ�, ���ɞ�NULL����ִ�
	//		char *szKey				//Key name�ִ�, ���ɞ�NULL����ִ�
	//		char *szValues			//Values���ִ�
	inline	void					SetPrivateProfileString( char *szSection, char *szKey, char *szValues );

	//ȡ��ָ��Section���Q�cKey���Q��Values(Int Type)
	//����:
	//		char *szSection			//Section name�ִ�, ���ɞ�NULL����ִ�
	//		char *szKey				//Key name�ִ�, ���ɞ�NULL����ִ�
	//����:
	//		Values��ֵ
	inline	int						GetPrivateProfileInt( char *szSection, char *szKey );

	//�O��ָ��Section���Q�cKey���Q��Values(Int Type)
	//����:
	//		char *szSection			//Section name�ִ�, ���ɞ�NULL����ִ�
	//		char *szKey				//Key name�ִ�, ���ɞ�NULL����ִ�
	//		int iValues				//Values��ֵ
	inline	void					SetPrivateProfileInt( char *szSection, char *szKey, int iValues );


	//�n�����P
	//�_��ָ���n��
	//����:
	//		char *szFileName		//�n��·�����Q
	//����:
	//		ʧ��FALSE, �ɹ�TRUE
	inline	BOOL					OpenFile( const char *szFileName );

	//����ָ���n��
	//����:
	//		char *szFileName		//�n��·�����Q
	//����:
	//		ʧ��FALSE, �ɹ�TRUE
	inline	BOOL					WriteFile( char *szFileName );

	//�_��ָ�����a�n��(���a��ӆ)
	//����:
	//		char	*szFileName		//�n��·�����Q
	//		int		iCodeCount		//���a����
	//		char	*pDeCode		//���a���Mλַ
	//����:
	//		ʧ��FALSE, �ɹ�TRUE
	inline	BOOL					OpenBinFile( char *szFileName, int iCodeCount, char *pDeCode );

	//����ָ�����a�n��(���a��ӆ)
	//����:
	//		char	*szFileName		//�n��·�����Q
	//		int		iCodeCount		//���a����
	//		char	*pEnCode		//���a���Mλַ
	//����:
	//		ʧ��FALSE, �ɹ�TRUE
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
//ȡ�� Key ���Q
inline
char*
PRIVATEPROFILEKEY::GetKey()
{
	return pKey;
}
//�O�� Key ���Q
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
//ȡ�� Values(String Type)
inline
char*
PRIVATEPROFILEKEY::GetValuesString()
{
	return pValues;
}
//�O�� Values(String Type)
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

//ȡ�� Values(Int Type)
inline
int
PRIVATEPROFILEKEY::GetValuesInt()
{
	if( pValues )
		return atoi( pValues );

	return 0;
}
//�O�� Values(Int Type)
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
//ȡ�� Key �Ĕ���
inline
int
PRIVATEPROFILESECTION::Count()
{
	return vKey.size();
}
//ȡ�� Section ���Q
inline
char*
PRIVATEPROFILESECTION::GetSection()
{
	return pSection;
}
//�O�� Section ���Q
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
//�h��ָ��������̖�� Key �
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
//�h��ָ�� Key ���Q�� Key �
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
//ȡ��ָ��������̖�� Key �
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
//ȡ��ָ�� Key ���Q�� Key �
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
//�O�� Key 헵� Value(String Type), ����]��ָ���� Key �, �t�a���� Key �, �K�O�� Key & Value
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
//�O�� Key 헵� Value(Int Type), ����]��ָ���� Key �, �t�a���� Key �, �K�O�� Key & Value
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
//ȡ�� Section �Ĕ���
inline
int
PRIVATEPROFILE::Count()
{
	return vSection.size();
}
//ȡ��ָ��������̖�� Section �
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
//ȡ��ָ�� Section ���Q�� Section �
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
//�O�� Section ���Q
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
//�h��ָ��������̖�� Section �
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
//�h��ָ�� Section ���Q�� Section �
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
//ȡ��ָ��Section���Q�cKey���Q��Values(String Type)
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
//�O��ָ��Section���Q�cKey���Q��Values(String Type)
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
//ȡ��ָ��Section���Q�cKey���Q��Values(Int Type)
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
//�O��ָ��Section���Q�cKey���Q��Values(Int Type)
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
//�_��ָ���n��
inline
BOOL
PRIVATEPROFILE::OpenFile( const char *szFileName )
{
	FILE	*hFile;
	int		iLineLength, iSectionLength, iKeyLength, iValuesLength;
	char	*pStrFind;		//������Ԫ
	char	*pTempBuffer;	//������
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
				{	//����Section Buffer
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
					{	//����Key Buffer
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
						{	//����Values Buffer
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
		{	//����Line Buffer
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
//����ָ���n��
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
//�_��ָ�����a�n��(���a��ӆ)
inline
BOOL
PRIVATEPROFILE::OpenBinFile( char *szFileName, int iCodeCount, char *pDeCode )
{
	FILE	*hFile;
	int		iLineLength, iDeCodeLoop;
	int		iSectionLength, iKeyLength, iValuesLength;
	char	*pStrFind;		//������Ԫ
	char	*pTempBuffer;	//������
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
				{	//����Section Buffer
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
					{	//����Key Buffer
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
						{	//����Values Buffer
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
		{	//����Line Buffer
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
//����ָ�����a�n��(���a��ӆ)
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
		{	//����Section Buffer
			delete [] pSectionBuffer;
			iMaxSectionBuffer += ( iSectionLength - iMaxSectionBuffer + 1 );
			pSectionBuffer = new char[ iMaxSectionBuffer ];
		}//if
		sprintf_s( pSectionBuffer, iSectionLength, "[%s]\n", pfSection->GetSection() );
		//Section�ִ����a
		for( i = 0; i < iSectionLength; i++ )
		{
			pSectionBuffer[i] = ( pSectionBuffer[ i ]  + pEnCode[ iDeCodeLoop ] );
			iDeCodeLoop = ( iDeCodeLoop + 1 ) % iCodeCount;
		}//for
		//����n��
		fwrite( pSectionBuffer, iSectionLength, 1, hFile );

		//Key
		for( iKey = 0; iKey < pfSection->Count(); iKey++ )
		{
			pfKey = pfSection->GetKey( iKey );
			iLineLength = strlen( pfKey->GetKey() ) + strlen( pfKey->GetValuesString() ) + 2;
			while( iMaxLineBuffer <= iLineLength )
			{	//����Line Buffer
				delete [] pLineBuffer;
				iMaxLineBuffer += 256;
				pLineBuffer = new char[ iMaxLineBuffer + 256 ];
			}//while
			sprintf_s( pLineBuffer, iSectionLength, "%s=%s\n", pfKey->GetKey(), pfKey->GetValuesString() );
			//Key&Values�ִ����a
			for( i = 0; i < iLineLength; i++ )
			{
				pLineBuffer[i] = ( pLineBuffer[ i ]  + pEnCode[ iDeCodeLoop ] );
				iDeCodeLoop = ( iDeCodeLoop + 1 ) % iCodeCount;
			}//for
			//����n��
			fwrite( pLineBuffer, iLineLength, 1, hFile );
		}//for
	}//for

	fclose( hFile );

	return TRUE;
}

#endif