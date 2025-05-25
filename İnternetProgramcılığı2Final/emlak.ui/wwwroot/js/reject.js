let currentPropertyId = null;

function loadRejectedProperties() {
    $.ajax({
        url: `${config.apiUrl}/api/admin/AdminProperty/rejected`,
        type: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        success: function(response) {
            if (response.status) {
                const properties = response.data;
                const tbody = $('#rejectedPropertiesTable tbody');
                tbody.empty();

                properties.forEach(property => {
                    // Lokasyon bilgilerini kontrol et
                    const cityName = property.city?.name || '';
                    const districtName = property.district?.name || '';
                    const locationText = cityName && districtName ? `${cityName} / ${districtName}` : '';

                    // Red tarihini formatla
                    const rejectedDate = property.rejectedAt ? 
                        new Date(property.rejectedAt).toLocaleDateString('tr-TR') : 
                        '';

                    tbody.append(`
                        <tr>
                            <td>${property.title || ''}</td>
                            <td>${property.price ? property.price.toLocaleString('tr-TR') : '0'} ₺</td>
                            <td>${locationText}</td>
                            <td>${property.squareMeters || '0'} m²</td>
                            <td>${property.roomCount || '0'} + 1</td>
                            <td>${rejectedDate}</td>
                            <td>
                                <button class="btn btn-sm btn-info" onclick="viewProperty(${property.id})">
                                    <i class="fas fa-eye"></i>
                                </button>
                            </td>
                        </tr>
                    `);
                });

                // DataTables'ı yeniden başlat
                if ($.fn.DataTable.isDataTable('#rejectedPropertiesTable')) {
                    $('#rejectedPropertiesTable').DataTable().destroy();
                }
                $('#rejectedPropertiesTable').DataTable({
                    language: {
                        url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/tr.json'
                    }
                });
            }
        },
        error: function(xhr) {
            console.error('Reddedilen ilanlar yüklenirken hata oluştu:', xhr);
            toastr.error('Reddedilen ilanlar yüklenirken bir hata oluştu');
        }
    });
}

function viewProperty(id) {
    currentPropertyId = id;
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
                $('#modalStatus').text('Reddedildi');
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

// Sayfa yüklendiğinde reddedilen ilanları yükle
$(document).ready(function() {
    loadRejectedProperties();
});
