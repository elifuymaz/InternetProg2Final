console.log("ahmet");

// Admin property management functions
function loadProperties() {
    $.ajax({
        url: `${config.apiUrl}/api/admin/AdminProperty/approved`,
        type: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        success: function(response) {
            if (response.status) {
                const properties = response.data;
                const tbody = $('#propertiesTable tbody');
                tbody.empty();

                properties.forEach(property => {
                    // Status badge'ini güncelle
                    let statusBadge = '';
                    if (property.status === "Active" && property.isApproved) {
                        statusBadge = '<span class="badge bg-success">Onaylı</span>';
                    } else if (property.status === "Rejected") {
                        statusBadge = '<span class="badge bg-danger">Reddedildi</span>';
                    } else {
                        statusBadge = '<span class="badge bg-warning">Onay Bekliyor</span>';
                    }

                    // Lokasyon bilgilerini kontrol et
                    const cityName = property.city?.name || '';
                    const districtName = property.district?.name || '';
                    const locationText = cityName && districtName ? `${cityName} / ${districtName}` : '';

                    tbody.append(`
                        <tr>
                            <td>${property.title || ''}</td>
                            <td>${property.price ? property.price.toLocaleString('tr-TR') : '0'} ₺</td>
                            <td>${locationText}</td>
                            <td>${property.squareMeters || '0'} m²</td>
                            <td>${property.roomCount || '0'} + 1</td>
                            <td>${statusBadge}</td>
                            <td>
                                <button class="btn btn-sm btn-info" onclick="viewProperty(${property.id})">
                                    <i class="fas fa-eye"></i>
                                </button>
                                <button class="btn btn-sm btn-danger" onclick="deleteProperty(${property.id})">
                                    <i class="fas fa-trash"></i>
                                </button>
                            </td>
                        </tr>
                    `);
                });

                // DataTables'ı yeniden başlat
                if ($.fn.DataTable.isDataTable('#propertiesTable')) {
                    $('#propertiesTable').DataTable().destroy();
                }
                $('#propertiesTable').DataTable({
                    language: {
                        url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/tr.json'
                    }
                });
            }
        },
        error: function(xhr) {
            console.error('İlanlar yüklenirken hata oluştu:', xhr);
            toastr.error('İlanlar yüklenirken bir hata oluştu');
        }
    });
}

function viewProperty(id) {
    $.ajax({
        url: `${config.apiUrl}/api/admin/AdminProperty/${id}`,
        type: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        success: function(response) {
            if (response.status) {
                const property = response.data;
                
                // İlan bilgilerini doldur
                $('#modalTitle').text(property.title);
                $('#modalPrice').text(property.price.toLocaleString('tr-TR') + ' ₺');
                $('#modalSquareMeters').text(property.squareMeters + ' m²');
                $('#modalRoomCount').text(property.roomCount + ' + 1');
                $('#modalBathroomCount').text(property.bathroomCount);
                $('#modalPropertyType').text(property.propertyType);
                $('#modalStatus').text(property.isApproved ? 'Onaylı' : 'Onay Bekliyor');
                $('#modalDescription').text(property.description);
                $('#modalAddress').text(property.address);
                $('#modalCity').text(property.city?.name || '');
                $('#modalDistrict').text(property.district?.name || '');

                // Özellikleri doldur
                const featuresDiv = $('#modalFeatures');
                featuresDiv.empty();
                if (property.features && property.features.length > 0) {
                    property.features.forEach(feature => {
                        featuresDiv.append(`
                            <span class="badge bg-primary">${feature.name}: ${feature.value}</span>
                        `);
                    });
                }

                // Resimleri doldur
                const imagesDiv = $('#modalImages');
                imagesDiv.empty();
                if (property.images && property.images.length > 0) {
                    property.images.forEach(image => {
                        // Resim URL'sini tam URL'ye çevir
                        const fullImageUrl = image.imageUrl.startsWith('/') 
                            ? `${config.apiUrl}${image.imageUrl}`
                            : image.imageUrl;
                            
                        imagesDiv.append(`
                            <img src="${fullImageUrl}" class="img-thumbnail" style="width: 150px; height: 150px; object-fit: cover;">
                        `);
                    });
                }

                $('#propertyModal').modal('show');
            }
        },
        error: function(xhr) {
            console.error('İlan detayları alınırken hata oluştu:', xhr);
            toastr.error('İlan detayları alınırken bir hata oluştu');
        }
    });
}

function deleteProperty(id) {
    if (confirm('Bu ilanı silmek istediğinizden emin misiniz?')) {
        $.ajax({
            url: `${config.apiUrl}/api/admin/AdminProperty/${id}`,
            type: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('token')
            },
            success: function(response) {
                if (response.status) {
                    loadProperties();
                    toastr.success('İlan başarıyla silindi');
                }
            },
            error: function(xhr) {
                console.error('İlan silinirken hata oluştu:', xhr);
                toastr.error('İlan silinirken bir hata oluştu');
            }
        });
    }
}

// Sayfa yüklendiğinde ilanları yükle
$(document).ready(function() {
    loadProperties();
});